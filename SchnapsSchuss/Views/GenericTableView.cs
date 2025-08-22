using System.Reflection;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public class GenericTableView<T> : ContentView
{

    private readonly CrudViewModel<T> _viewModel;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSourceProperty), 
            typeof(IEnumerable<T>), 
            typeof(GenericTableView<T>), 
            null, 
            propertyChanged: OnItemsSourceChanged);
    
    public IEnumerable<T> ItemsSource
    {
        get => (IEnumerable<T>)GetValue(ItemsSourceProperty);
        init => SetValue(ItemsSourceProperty, value);
    }
    
    public GenericTableView(CrudViewModel<T> viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GenericTableView<T> tableView)
        {
            tableView.BuildTable();
        }
    }

    private void BuildTable()
    {
        var properties = _viewModel.GetProperties();

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                properties.Select(_ => new ColumnDefinition { Width = GridLength.Star }).ToArray()
            )
        };

        // Header Row
        for (var i = 0; i < properties.Length; i++)
        {
            grid.Add(new Label
            {
                Text = properties[i].Name, 
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
            }, i, 0);
        }

        // Data Rows
        var row = 1;
        foreach (var item in ItemsSource)
        {
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
                    BackgroundColor = (Color) (row % 2 == 0 ? Application.Current?.Resources["OffBlack"] :  Application.Current?.Resources["Gray500"])!,
                }, col, row);
            }
            row++;
        }
        Content = new ScrollView
        {
            Content = grid,
        };
    }
}
