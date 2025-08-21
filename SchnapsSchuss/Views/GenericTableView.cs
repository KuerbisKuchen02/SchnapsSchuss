using System.Reflection;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public class GenericTableView<T> : ContentView
{

    private CrudViewModel<T> _viewModel;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource), 
            typeof(IEnumerable<T>), 
            typeof(GenericTableView<T>), 
            null, 
            propertyChanged: OnItemsSourceChanged);

    public GenericTableView(CrudViewModel<T> viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
    
    public IEnumerable<T> ItemsSource
    {
        get => (IEnumerable<T>)GetValue(ItemsSourceProperty);
        init => SetValue(ItemsSourceProperty, value);
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
        var properties = typeof(T).GetProperties()
            .Where(property => _viewModel.ShownColumnNames.Contains(property.Name)).ToArray();

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                properties.Select(_ => new ColumnDefinition { Width = GridLength.Star }).ToArray()
            )
        };

        // Header Row
        for (var i = 0; i < properties.Length; i++)
        {
            grid.Add(new Label { Text = properties[i].Name, FontAttributes = FontAttributes.Bold }, i, 0);
        }

        // Data Rows
        var row = 1;
        foreach (var item in ItemsSource)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (var col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item)?.ToString() ?? string.Empty;
                grid.Add(new Label
                {
                    Text = value,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(5, 2),
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center
                }, col, row);
            }

            row++;
        }
        Content = new ScrollView { Content = grid };
    }
}
