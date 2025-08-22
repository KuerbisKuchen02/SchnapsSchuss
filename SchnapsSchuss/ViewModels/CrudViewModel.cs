using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class CrudViewModel<T>(string title, List<string> shownColumn) : BaseViewModel
{
    private readonly Database<T> _database = DatabaseFactory<T>.GetDatabase();

    public ObservableCollection<T> Items { get; set; } = [];
    public ObservableCollection<T> FilteredItems { get; set; } = [];
    
    private string _title = title;
    
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

    private List<string> _shownColumnNames = shownColumn;

    public List<string> ShownColumnNames
    {
        get => _shownColumnNames;
        set => SetProperty(ref _shownColumnNames, value);
    }

    public ICommand AddItem => new Command(OnAddButtonClicked);
    public ICommand EditItem => new Command(OnRowClicked);
    public ICommand ClosePopUp => new Command(OnPopupCancel);
    public ICommand PerformSearch => new Command<string>(FilterTable);

    public PropertyInfo[] GetProperties()
    {
        return typeof(T).GetProperties()
            .Where(property => _shownColumnNames.Contains(property.Name)).ToArray();
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
    }

    private void FilterTable(string query)
    {
        List<T> filteredItems = [];
        filteredItems.AddRange(
            from item in Items 
            from prop in GetProperties() 
            where (prop.GetValue(item)?.ToString() ?? "").Contains(query)
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