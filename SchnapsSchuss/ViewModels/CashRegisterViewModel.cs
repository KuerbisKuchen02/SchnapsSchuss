using CommunityToolkit.Mvvm.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

public class CashRegisterViewModel : BaseViewModel, IQueryAttributable
{
    private readonly InvoiceDatabase _invoiceDb;
    private readonly InvoiceItemDatabase _invoiceItemDb;
    private readonly ArticleDatabase _articleDb;

    private readonly List<Article> _allArticles = [];
    public ObservableCollection<ArticleViewModel> FilteredArticles { get; set; } = [];

    public string PersonLabel => Invoice?.Person != null
        ? $"Person - {Invoice.Person.FirstName} {Invoice.Person.LastName}"
        : "Person -";

    // Current Invoice
    private Invoice? _invoice;

    private Invoice Invoice
    {
        get => _invoice ?? new Invoice();
        set
        {
            SetProperty(ref _invoice, value);
            OnPropertyChanged(nameof(PersonLabel));
        }
    }

    // Total is calculated by aggregating InvoiceItems
    public float InvoiceTotal => InvoiceItems.Sum(i => i.TotalPrice);

    private ObservableCollection<InvoiceItem> _invoiceItems = [];

    public ObservableCollection<InvoiceItem> InvoiceItems
    {
        get => _invoiceItems;
        set => SetProperty(ref _invoiceItems, value);
    }

    // Current ArticleType used for filtering
    private ArticleType _curArticleType;

    public ArticleType CurArticleType
    {
        get => _curArticleType;
        set => SetProperty(ref _curArticleType, value);
    }

    // Commands for UI Buttons
    public ICommand AddArticleCommand { get; private set; }
    public ICommand FilterArticlesCommand { get; private set; }
    public ICommand PayInvoiceCommand { get; private set; }
    public IAsyncRelayCommand BackCommand { get; }

    public CashRegisterViewModel()
    {
        _articleDb = new ArticleDatabase();
        _invoiceDb = new InvoiceDatabase();
        _invoiceItemDb = new InvoiceItemDatabase();

        AddArticleCommand = new Command<Article>(AddArticleToInvoice);
        FilterArticlesCommand = new Command<ArticleType>(FilterArticlesByType);
        PayInvoiceCommand = new Command(PayInvoice);
        BackCommand = new AsyncRelayCommand(BackAsync);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await InitAsync((Person)query["Person"]);
    }

    private async Task InitAsync(Person person)
    {
        await LoadArticlesAsync();
        await LoadInvoiceAsync(person);
        FilterArticlesByType(ArticleType.DRINK);
    }

    public async void OnDisappearing()
    {
        if (Invoice.InvoiceItems.Count != 0) return;

        Debug.WriteLine("No items in invoice, deleting invoice");
        await _invoiceDb.DeleteAsync(Invoice);
    }

    private async Task LoadArticlesAsync()
    {
        var articles = await _articleDb.GetAllAsync();
        foreach (var article in articles)
            _allArticles.Add(article);
    }

    private async Task LoadInvoiceAsync(Person person)
    {
        var openInvoice = await _invoiceDb.GetOpenInvoiceForPerson(person.Id);

        // If there is an open Invoice, show it. If not, create a new Invoice.
        if (openInvoice is not null)
        {
            var openInvoiceItems = await _invoiceItemDb.GetInvoiceItemsOfInvoiceAsync(openInvoice);
            openInvoice.InvoiceItems = openInvoiceItems;
            foreach (InvoiceItem item in openInvoiceItems)
            {
                item.Article = await new ArticleDatabase().GetOneAsync(item.ArticleId);
            }
            openInvoice.Person = person;
            Invoice = openInvoice;
            InvoiceItems = new ObservableCollection<InvoiceItem>(openInvoice.InvoiceItems);
        }
        else
        {
            Invoice = new Invoice
            {
                Date = DateTime.Now,
                isPaidFor = false,
                InvoiceItems = [],
                Person = person,
                PersonId = person.Id
            };
        }

        OnPropertyChanged(nameof(InvoiceItems));
        OnPropertyChanged(nameof(InvoiceTotal));
    }

    private void AddArticleToInvoice(Article article)
    {
        // To add articles to an invoice, first check if the article is already contained in the invoice.
        var existingInvoiceItem = InvoiceItems
            .FirstOrDefault(i => i.ArticleId == article.Id);

        // Check if the article has stock left
        if (article.Stock is 0 or < -1)
            return;

        // Determine the correct price according to the role
        var articlePrice = Invoice.Person.Role == RoleType.GUEST ? article.PriceGuest : article.PriceMember;

        // Article already exists. Just increase the amount and re-calculate the price.
        if (existingInvoiceItem != null)
        {
            existingInvoiceItem.Amount++;
            existingInvoiceItem.TotalPrice = existingInvoiceItem.Amount * articlePrice;
            OnPropertyChanged(nameof(existingInvoiceItem.Amount));
        }
        else
            // Article new, create a new InvoiceItem with the article.
        {
            InvoiceItem newItem = new()
            {
                Article = article,
                ArticleId = article.Id,
                Amount = 1,
                TotalPrice = articlePrice
            };
            InvoiceItems.Add(newItem);
        }

        // Update InvoiceTotal
        OnPropertyChanged(nameof(InvoiceTotal));

        if (article.Stock <= 0) return;
        
        article.Stock--;
        _ = _articleDb.SaveAsync(article);
    }
    
    private void FilterArticlesByType(ArticleType articleType)
    {
        CurArticleType = articleType;

        // Wrap articles in ArticleViewModel to show the price according to the current invoice
        var filtered = _allArticles
            .Where(a => a.Type == articleType)
            .Select(a => new ArticleViewModel(a, Invoice))
            .ToList();

        // Clear and repopulate FilteredArticles
        FilteredArticles.Clear();
        foreach (var articleVm in filtered)
        {
            FilteredArticles.Add(articleVm);
        }
    }

    private async void PayInvoice()
    {
        Invoice.isPaidFor = true;

        await BackAsync();

        await Shell.Current.GoToAsync("..");
    }

    private async Task BackAsync()
    {
        Invoice.InvoiceItems = [.. InvoiceItems];
        await _invoiceDb.SaveAsync(Invoice);
        await _invoiceItemDb.SaveInvoiceItemsAsync([.. InvoiceItems]);
    }
}