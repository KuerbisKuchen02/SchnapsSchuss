using System.Collections.ObjectModel;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.ViewModels;

public class CashRegisterViewModel : BaseViewModel
{
    private InvoiceDatabase _invoiceDb;
    private ArticleDatabase _articleDb;

    public ObservableCollection<Article> AllArticles { get; } = new ObservableCollection<Article>();
    public ObservableCollection<Article> FilteredArticles { get; set; } = new ObservableCollection<Article>();

    // Current Person
    private Person _Person;
    public Person Person
    {
        get => _Person;
        set => SetProperty(ref _Person, value);
    }

    // Current Invoice
    private Invoice _Invoice;
    public Invoice Invoice
    {
        get => _Invoice;
        set => SetProperty(ref _Invoice, value);
    }

    // Current ArticleType used for filtering
    private ArticleType _curArticleType;
    public ArticleType CurArticleType
    {
        get => _curArticleType;
        set => SetProperty(ref _curArticleType, value);
    }

    // Commands for UI Buttons
    public ICommand AddArticleCommand { get; }
    public ICommand FilterArticlesCommand { get; }

    public CashRegisterViewModel(Person person)
    {
        _Person = person;

        _articleDb = new ArticleDatabase();
        _invoiceDb = new InvoiceDatabase();

        InitAsync();

        AddArticleCommand = new Command<Article>(AddArticleToInvoice);
        FilterArticlesCommand = new Command<ArticleType>(FilterArticlesByType);

        FilterArticlesByType(ArticleType.DRINK);  // Default category is always Drink
    }

    private async Task InitAsync()
    {
        await LoadArticlesAsync();
        await LoadInvoiceAsync();
    }

    private async Task LoadArticlesAsync()
    {
        List<Article> articles = await _articleDb.GetArticlesAsync();
        foreach (var article in articles)
            AllArticles.Add(article);
    }

    private async Task LoadInvoiceAsync()
    {
        List<Invoice> openInvoices = await _invoiceDb.GetInvoicesAsync();
        openInvoices = openInvoices.Where(i => i.isPaidFor == false).ToList();

        // If there is an open Invoice, show it. If not, create a new Invoice.
        if (openInvoices.Count() == 1) _Invoice = openInvoices[0];
        else if (openInvoices.Count == 0)
        {
            _Invoice = new Invoice
            {
                Date = DateTime.Now,
                isPaidFor = false,
                Person = _Person,
                PersonId = _Person.Id,
                invoiceItems = new List<InvoiceItem>()
            };
        }
    }

    private void AddArticleToInvoice(Article article)
    {
        // To add articles to an invoice, first check if the article is alreay contained in the invoice.
        InvoiceItem ExistingInvoiceItem = _Invoice.invoiceItems
            .FirstOrDefault(i => i.Article.Id == article.Id);

        // Article already exists. Just increase the amount and re-calculate the price.
        if (ExistingInvoiceItem != null)
        {
            ExistingInvoiceItem.Amount++;
            ExistingInvoiceItem.TotalPrice = ExistingInvoiceItem.Amount * article.PriceMember;
        }
        else
        // Article new, create a new InvoiceItem with the article.
        {
            InvoiceItem newItem = new InvoiceItem
            {
                Article = article,
                Amount = 1,
                TotalPrice = article.PriceMember
            };
            _Invoice.invoiceItems.Add(newItem);
        }
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

    private void PayInvoice()
    {
        _Invoice.isPaidFor = true;
    }

    public void CloseView()
    {
        _invoiceDb.SaveInvoiceAsync(_Invoice);
    }
}