using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class CrudViewModel<T> : BaseViewModel
{
    private readonly Database<T> _database;
    private readonly ArticleType? _articleType;

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
    
    // ReSharper disable once UnusedMember.Global
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
    // ReSharper disable once UnusedMember.Global
    public T SelectedItem
    {
        get => _selectedItem ?? Activator.CreateInstance<T>();
        set => SetProperty(ref _selectedItem, value);
    }


    private bool _isDeletable;
    public bool IsDeletable
    {
        get => _isDeletable;
        set => SetProperty(ref _isDeletable, value);
    }

    private IDictionary<string, string> _shownColumnNames;
    // ReSharper disable once UnusedMember.Global
    public IDictionary<string, string> ShownColumnNames
    {
        get => _shownColumnNames;
        set => SetProperty(ref _shownColumnNames, value);
    }
    
    private string _searchText;

    // ReSharper disable once UnusedMember.Global
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value)) FilterTable(value);
        }
    }

    // ReSharper disable once UnusedMember.Global
    public ICommand AddItem => new Command(OnAddButtonClicked);
    // ReSharper disable once UnusedMember.Global
    public ICommand CancelEdit => new Command(OnPopupCancel);
    // ReSharper disable once UnusedMember.Global
    public ICommand SaveEdit => new Command(OnPopupSubmit);
    // ReSharper disable once UnusedMember.Global
    public ICommand DeleteSelected => new Command(OnDelete);
    
    public PropertyInfo[] GetProperties()
    {
        if (_shownColumnNames.Count == 0) return typeof(T).GetProperties();
        
        return typeof(T).GetProperties()
            .Where(property => _shownColumnNames.ContainsKey(property.Name)).ToArray();
    }

    public CrudViewModel(IDictionary<string, object> query)
    {
        query.TryGetValue("title", out var t);
        _title = t?.ToString() ?? "unknown";
        query.TryGetValue("shownColumns", out var c);
        _shownColumnNames = c != null ? (IDictionary<string, string>) c : new Dictionary<string, string>();
        query.TryGetValue("articleType", out var aType);
        _articleType = (ArticleType?) aType;
        
        _searchText = string.Empty;
        _database = DatabaseFactory<T>.GetDatabase();
        LoadItems();
    }

    public async void OnRowClicked(T entity)
    {
        SelectedItem = entity;
        IsDeletable = true;
        PopupTitle = _title + " bearbeiten";
        ShowPopup();
    }
    
    private async void LoadItems()
    {
        var items = await _database.GetAllAsync();
        items.RemoveAll((item) =>
        {
            if (item is Article article)
            {
                return article.Type != _articleType;
            }
            return false;
        });
        Items = new ObservableCollection<T>(items);
        FilteredItems = new ObservableCollection<T>(Items);
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
        SelectedItem = Activator.CreateInstance<T>();
        IsDeletable = false;
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
    
    private void OnPopupSubmit()
    {
        if (SelectedItem is Article article && _articleType != null)
        {
            article.Type = (ArticleType)_articleType;
        }
        _database.SaveAsync(SelectedItem);
        Shell.Current.CurrentPage.ClosePopupAsync();
        LoadItems();
    }

    private void OnDelete()
    {
        _database.DeleteAsync(SelectedItem);
        Shell.Current.CurrentPage.ClosePopupAsync();
        LoadItems();
    }
}