using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

public class CashRegisterViewModel : BaseViewModel, IQueryAttributable
{
    private InvoiceDatabase _invoiceDb;
    private InvoiceItemDatabase _invoiceItemDb;
    private ArticleDatabase _articleDb;

    public ObservableCollection<Article> AllArticles { get; } = [
        ];
    public ObservableCollection<ArticleViewModel> FilteredArticles { get; set; } = new();

    public string PersonLabel =>
    Invoice?.Person != null
        ? $"Person - {Invoice.Person.FirstName} {Invoice.Person.LastName}"
        : "Person -";

    // Current Invoice
    private Invoice _invoice;

    public Invoice Invoice
    {
        get => _invoice;
        set
        {
            SetProperty(ref _invoice, value);
            OnPropertyChanged(nameof(PersonLabel));
        }
    }

    // Total is calculated by aggregating InvoiceItems
    public float InvoiceTotal => InvoiceItems.Sum(i => i.TotalPrice);


    private ObservableCollection<InvoiceItem> _invoiceItems;

    public ObservableCollection<InvoiceItem> InvoiceItems
    {
        get => _invoiceItems ??= new ObservableCollection<InvoiceItem>(_invoice?.InvoiceItems ?? Enumerable.Empty<InvoiceItem>());
        set
        {
            SetProperty(ref _invoiceItems, value);
        }
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

    private async Task InitAsync(Person person)
    {
        await LoadArticlesAsync();
        await LoadInvoiceAsync(person);
        FilterArticlesByType(ArticleType.DRINK);
    }

    public async void OnDisappearing()
    {
        if(Invoice.InvoiceItems.Count == 0)
            await _invoiceDb.DeleteAsync(Invoice);
    }

    private async Task LoadArticlesAsync()
    {
        List<Article> articles = await _articleDb.GetAllAsync();
        foreach (var article in articles)
            AllArticles.Add(article);
    }

    private async Task LoadInvoiceAsync(Person person)
    {
        Invoice openInvoice = await _invoiceDb.GetOpenInvoiceForPerson(person.Id);

        // If there is an open Invoice, show it. If not, create a new Invoice.
        if (openInvoice is not null)
        {
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
        OnPropertyChanged(nameof(InvoiceTotal));
    }

    private void AddArticleToInvoice(Article article)
    {
        // To add articles to an invoice, first check if the article is alreay contained in the invoice.
        InvoiceItem ExistingInvoiceItem = InvoiceItems
            .FirstOrDefault(i => i.Article.Id == article.Id);

        // Check if the article has stock left
        if (article.Stock <= 0)
            return;

        // Determine the correct price accroding to the role
        float articlePrice = Invoice.Person.Role == RoleType.GUEST ? article.PriceGuest : article.PriceMember;

        // Article already exists. Just increase the amount and re-calculate the price.
        if (ExistingInvoiceItem != null)
        {
            ExistingInvoiceItem.Amount++;
            ExistingInvoiceItem.TotalPrice = ExistingInvoiceItem.Amount * articlePrice;
            OnPropertyChanged(nameof(ExistingInvoiceItem.Amount));
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
        subtractArticleAmount(article);
    }

    private async void subtractArticleAmount(Article article)
    {
        article.Stock--;
        _articleDb.SaveAsync(article);
    }

    private void FilterArticlesByType(ArticleType articleType)
    {
        CurArticleType = articleType;

        // Wrap articles in ArticleViewModel to show the price according to the current invoice
        var filtered = AllArticles
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

    public async void PayInvoice()
    {
        Invoice.isPaidFor = true;

        await BackAsync();
    }

    private async Task BackAsync()
    {
        Invoice.InvoiceItems = [.. InvoiceItems];
        await _invoiceDb.SaveAsync(Invoice);
        await _invoiceItemDb.SaveInvoiceItemsAsync([.. InvoiceItems]);

        return;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await InitAsync(((Person)query["Person"]));
    }
}