using CommunityToolkit.Mvvm.ComponentModel;
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
    public ObservableCollection<Article> FilteredArticles { get; set; } = [
        ];

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
    public ICommand CloseViewCommand => new Command(CloseView);


    public CashRegisterViewModel() 
    {
        _articleDb = new ArticleDatabase();
        _invoiceDb = new InvoiceDatabase();
        _invoiceItemDb = new InvoiceItemDatabase();

        AddArticleCommand = new Command<Article>(AddArticleToInvoice);
        FilterArticlesCommand = new Command<ArticleType>(FilterArticlesByType);
        PayInvoiceCommand = new Command(PayInvoice);
    }

    private async Task InitAsync(Person person)
    {
        await LoadArticlesAsync();
        await LoadInvoiceAsync(person);
        FilterArticlesByType(ArticleType.DRINK);
    }

    private async Task LoadArticlesAsync()
    {
        List<Article> articles = await _articleDb.GetArticlesAsync();
        foreach (var article in articles)
            AllArticles.Add(article);
    }

    private async Task LoadInvoiceAsync(Person person)
    {
        List<Invoice> openInvoices = await _invoiceDb.GetInvoicesAsync();
        openInvoices = openInvoices.Where(i => i.isPaidFor == false && i.PersonId == person.Id).ToList();

        // If there is an open Invoice, show it. If not, create a new Invoice.
        if (openInvoices.Count() == 1)
        {
            Invoice = openInvoices[0];
            InvoiceItems = new ObservableCollection<InvoiceItem>(openInvoices[0].InvoiceItems);
        }
        else if (openInvoices.Count == 0)
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
    }

    private void AddArticleToInvoice(Article article)
    {
        // To add articles to an invoice, first check if the article is alreay contained in the invoice.
        InvoiceItem ExistingInvoiceItem = InvoiceItems
            .FirstOrDefault(i => i.Article.Id == article.Id);

        // Article already exists. Just increase the amount and re-calculate the price.
        if (ExistingInvoiceItem != null)
        {
            ExistingInvoiceItem.Amount++;
            ExistingInvoiceItem.TotalPrice = ExistingInvoiceItem.Amount * article.PriceMember;
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
                TotalPrice = article.PriceMember
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
        _articleDb.SaveArticleAsync(article);
    }

    private void FilterArticlesByType(ArticleType articleType)
    {
        CurArticleType = articleType;
        var filtered = AllArticles.Where(a => a.Type == articleType).ToList();

        // Reset the FilteredArticles and repopulate it according to the ArticleType
        FilteredArticles.Clear();
        foreach (var article in filtered)
        {
            FilteredArticles.Add(article);
        }
    }

    public void PayInvoice()
    {
        Invoice.isPaidFor = true;

        CloseView();
    }

    public async void CloseView()
    {
        Invoice.InvoiceItems = [.. InvoiceItems];

        await _invoiceDb.SaveInvoiceAsync(Invoice);
        await _invoiceItemDb.SaveInvoiceItemsAsync([.. InvoiceItems]);

        await Shell.Current.GoToAsync("..");
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await InitAsync(((Person)query["Person"]));
    }
}