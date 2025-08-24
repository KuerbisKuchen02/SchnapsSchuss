using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public class GenericPropertyListView<T> : ContentView
{
    private readonly CrudViewModel<T> _viewModel;

    public GenericPropertyListView(CrudViewModel<T> viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Content = BuildList();
    }
    
    private Grid BuildList()
    {
        var properties = _viewModel.GetProperties();

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star }, 
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        var row = 0;
        foreach (var property in properties)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            grid.Add(new Label
            {
                Text = _viewModel.ShownColumnNames[property.Name], 
                FontSize = 18,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, row);
            
            var pType = property.PropertyType;

            View? view;
            var binding = new Binding(
                path: $"SelectedItem.{property.Name}",
                mode: BindingMode.TwoWay
            );
            if (pType == typeof(bool))
            {
                view = new Switch();
                view.SetBinding(Switch.IsToggledProperty, binding);
            } else if (pType == typeof(DateTime))
            {
                view = new DatePicker();
                view.SetBinding(DatePicker.DateProperty, binding);
            } else if (pType.IsEnum)
            {
                view = new Picker
                {
                    ItemsSource = Enum.GetNames(pType)
                };
                view.SetBinding(Picker.SelectedIndexProperty, binding);
            } else
            {
                var entry = new Entry();
                if (pType == typeof(int))
                {
                    entry.Placeholder = "Ganzzahl eingeben";
                    entry.Keyboard = Keyboard.Numeric;
                } else if (pType == typeof(float) || pType == typeof(double) || pType == typeof(decimal))
                {
                    entry.Placeholder = "Kommazahl eingeben";
                    entry.Keyboard = Keyboard.Numeric;
                }

                entry.SetBinding(Entry.TextProperty, binding);
                view = entry;
            }
            view.Margin = new Thickness(0, 10, 0, 0);
            grid.Add(view, 1, row);
            row++;
        }
        return grid;
    }
}