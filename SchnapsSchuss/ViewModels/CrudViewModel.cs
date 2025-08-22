using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class CrudViewModel<T> : BaseViewModel
{
    private readonly Database<T> _database;

    // Backing field is required to also notify when set to new object references  
    private ObservableCollection<T> _items = [];
    public ObservableCollection<T> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    private ObservableCollection<T> _filteredItems = [];
    public ObservableCollection<T> FilteredItems
    {
        get => _filteredItems;
        set => SetProperty(ref _filteredItems, value);
    }
    
    private string _title;
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string? PopupTitle
    {
        get;
        set;
    }
    
    private T? _selectedItem;

    public T SelectedItem
    {
        get => _selectedItem ?? Activator.CreateInstance<T>();
        set => SetProperty(ref _selectedItem, value);
    }

    private List<string> _shownColumnNames;

    public List<string> ShownColumnNames
    {
        get => _shownColumnNames;
        set => SetProperty(ref _shownColumnNames, value);
    }
    
    private string _searchText;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value)) FilterTable(value);
        }
    }

    public ICommand AddItem => new Command(OnAddButtonClicked);
    public ICommand EditItem => new Command(OnRowClicked);
    public ICommand ClosePopUp => new Command(OnPopupCancel);

    public PropertyInfo[] GetProperties()
    {
        return typeof(T).GetProperties()
            .Where(property => _shownColumnNames.Contains(property.Name)).ToArray();
    }

    public CrudViewModel(string title, List<string> shownColumn)
    {
        _title = title;
        _shownColumnNames = shownColumn;
        _database = DatabaseFactory<T>.GetDatabase();
        Init();
    }

    public void OnRowClicked()
    {
        PopupTitle = _title + " bearbeiten";
        ShowPopup();
    }
    
    private async void Init()
    {
        var items = await _database.GetAllAsync();
        Items = new ObservableCollection<T>(items);
        FilteredItems = new ObservableCollection<T>(Items);
        Console.Write("Hello");
    }

    private void FilterTable(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            FilteredItems = new ObservableCollection<T>(Items);
            return;
        }
        List<T> filteredItems = [];
        filteredItems.AddRange(
            from item in Items 
            from prop in GetProperties() 
            where (prop.GetValue(item)?.ToString()?.ToLower() ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase)
            select item);
        FilteredItems = filteredItems.ToObservableCollection();
    }


    private void OnAddButtonClicked()
    {
        PopupTitle = _title + " hinzufügen";
        ShowPopup();
    }

    private void ShowPopup()
    {
        Shell.Current.ShowPopupAsync(new EditEntityPopup()
        {
            BindingContext = this,
            EditorContent = new GenericPropertyListView<T>(this)
        });
    }

    private void OnPopupCancel()
    {
        Shell.Current.CurrentPage.ClosePopupAsync();
    }
}