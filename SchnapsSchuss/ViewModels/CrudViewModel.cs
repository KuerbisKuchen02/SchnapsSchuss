using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.ViewModels;

public class CrudViewModel<T> : BaseViewModel
{
    private List<T> _items;

    public List<T> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }
    
    private string _title;
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    private List<string> _shownColumnNames;

    public List<string> ShownColumnNames
    {
        get => _shownColumnNames;
        set => SetProperty(ref _shownColumnNames, value);
    }

    public CrudViewModel(string title, List<string> shownColumn)
    {
        _title = title;
        _items = [];
        _shownColumnNames = shownColumn;
    }
}