using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.ViewModels;

public class CrudViewModel<T>
{
    private readonly Database<T> _database;
    private readonly ArticleType? _articleType;

    // Backing field is required to also notify when set to new object references  
    public List<T> Items = [];
    public List<T> FilteredItems = [];
    public string Title;
    
    // ReSharper disable once UnusedMember.Global

    public string? PopupTitle { get; set; }
    
    private T? _selectedItem;
    // ReSharper disable once UnusedMember.Global
    public T SelectedItem { get; set; }


    private bool _isDeletable;
    public bool IsDeletable { get; set; }

    private IDictionary<string, string> _shownColumnNames;
    // ReSharper disable once UnusedMember.Global
    public IDictionary<string, string> ShownColumnNames { get; set; }
    
    private string _searchText;

    // ReSharper disable once UnusedMember.Global
    public string SearchText { get; set; }
    
    public PropertyInfo[] GetProperties()
    {
        if (_shownColumnNames.Count == 0) return typeof(T).GetProperties();
        
        return typeof(T).GetProperties()
            .Where(property => _shownColumnNames.ContainsKey(property.Name)).ToArray();
    }

    public CrudViewModel(IDictionary<string, object> query, Database<T> db)
    {
        query.TryGetValue("title", out var t);
        Title = t?.ToString() ?? "unknown";
        query.TryGetValue("shownColumns", out var c);
        _shownColumnNames = c != null ? (IDictionary<string, string>) c : new Dictionary<string, string>();
        query.TryGetValue("articleType", out var aType);
        _articleType = (ArticleType?) aType;
        
        _searchText = string.Empty;
        _database = db;  // Do not use DatabaseFactory
        LoadItems();
    }

    public async void OnRowClicked(T entity)
    {
        SelectedItem = entity;
        IsDeletable = true;
        PopupTitle = Title + " bearbeiten";
        ShowPopup();
    }
    
    public async void LoadItems()
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
        Items = new List<T>(items);
        FilteredItems = new List<T>(Items);
    }

    private void FilterTable(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            FilteredItems = new List<T>(Items);
            return;
        }
        List<T> filteredItems = [];
        filteredItems.AddRange(
            from item in Items 
            from prop in GetProperties() 
            where (prop.GetValue(item)?.ToString()?.ToLower() ?? "").Contains(query, StringComparison.CurrentCultureIgnoreCase)
            select item);
        FilteredItems = filteredItems;
    }


    private void OnAddButtonClicked()
    {
        SelectedItem = Activator.CreateInstance<T>();
        IsDeletable = false;
        PopupTitle = Title + " hinzufügen";
        ShowPopup();
    }

    private void ShowPopup()
    {
        // Shell.Current.ShowPopupAsync(new EditEntityPopup()
        // {
        //    BindingContext = this,
        //    EditorContent = new GenericPropertyListView<T>(this)
        // });
    }

    private void OnPopupCancel()
    {
        // Shell.Current.CurrentPage.ClosePopupAsync();
    }
    
    public void OnPopupSubmit()
    {
        if (SelectedItem is Article article && _articleType != null)
        {
            article.Type = (ArticleType)_articleType;
        }
        _database.SaveAsync(SelectedItem);
        // Shell.Current.CurrentPage.ClosePopupAsync();
        LoadItems();
    }

    public void OnDelete()
    {
        _database.DeleteAsync(SelectedItem);
        // Shell.Current.CurrentPage.ClosePopupAsync();
        LoadItems();
    }
}