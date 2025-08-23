using System.Collections.Specialized;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public class GenericTableView<T> : ContentView
{

    private readonly CrudViewModel<T> _viewModel;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource), 
            typeof(IEnumerable<T>), 
            typeof(GenericTableView<T>), 
            propertyChanged: OnItemsSourceChanged);
    
    public IEnumerable<T> ItemsSource
    {
        get => (IEnumerable<T>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    public GenericTableView(CrudViewModel<T> viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (GenericTableView<T>)bindable;

        if (oldValue is INotifyCollectionChanged oldObs)
            oldObs.CollectionChanged -= control.OnCollectionChanged!;

        if (newValue is INotifyCollectionChanged newObs)
            newObs.CollectionChanged += control.OnCollectionChanged!;

        control.BuildTable();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Sicher auf dem UI-Thread neu rendern
        MainThread.BeginInvokeOnMainThread(BuildTable);
    }

    private void BuildTable()
    {
        var properties = _viewModel.GetProperties();

        var root = new VerticalStackLayout();
        
        var header = new Grid() 
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                properties.Select(_ => new ColumnDefinition { Width = GridLength.Star }).ToArray()
            )
        };
        for (var i = 0; i < properties.Length; i++)
        {
            header.Add(new Label
            {
                Text = _viewModel.ShownColumnNames[properties[i].Name], 
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
            }, i);
        }
        root.Children.Add(header);

        var row = 1;
        foreach (var item in ItemsSource)
        {
            var grid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    properties.Select(_ => new ColumnDefinition { Width = GridLength.Star }).ToArray()
                )
            };
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });

            for (var col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item)?.ToString() ?? string.Empty;
                grid.Add(new Label
                {
                    Text = value,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18,
                    Margin = new Thickness(1, 5),
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center,
                    BackgroundColor = (Color) (row % 2 == 0 
                        ? Application.Current?.Resources["OffBlack"] 
                        : Application.Current?.Resources["Gray500"])!,
                }, col);
            }
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (_, _) =>
            {
                _viewModel.OnRowClicked(item);
            };
            grid.GestureRecognizers.Add(tapGestureRecognizer);
            root.Children.Add(grid);
            row++;
        }
        Content = new ScrollView
        {
            Content = root,
        };
    }
}
