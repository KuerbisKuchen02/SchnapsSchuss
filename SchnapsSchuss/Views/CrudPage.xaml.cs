using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public partial class CrudPage : IQueryAttributable
{
    
    public CrudPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var title = query["title"].ToString() ?? "unknown";
        var modelType = (Type) query["modelType"];
        var columns = (List<string>) query["shownColumns"];
        
        var vmType = typeof(CrudViewModel<>).MakeGenericType(modelType);
        var vm = Activator.CreateInstance(vmType, title, columns);
        BindingContext = vm;

        var tableType = typeof(GenericTableView<>).MakeGenericType(modelType);
        var tableView = (View)(Activator.CreateInstance(tableType, BindingContext) ?? throw new NullReferenceException());

        var bindable = (BindableProperty) tableType.GetField("ItemsSourceProperty")!.GetValue(null)!; 
        
        tableView.SetBinding(bindable, "FilteredItems");
        
        Table.Content = tableView;
    }
}