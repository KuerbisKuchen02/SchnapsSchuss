using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class CashRegisterViewModel : BaseViewModel
{
    private InvoiceDatabase _invoiceDb;
    private ArticleDatabase _articleDb;

    public ObservableCollection<Article> AllArticles { get; } = [
        ];
    public ObservableCollection<Article> FilteredArticles { get; set; } = [
        ];

    public string PersonLabel => $"Person - {_Invoice.Person.FirstName} {_Invoice.Person.LastName}";

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
    public ICommand AddArticleCommand { get; private set; }
    public ICommand FilterArticlesCommand { get; private set; }
    public ICommand PayInvoiceCommand { get; private set; }


    public CashRegisterViewModel()
    {
        _articleDb = new ArticleDatabase();
        _invoiceDb = new InvoiceDatabase();

        // Test Invoice for testing purposes
        _Invoice = new()
        {
            Date = DateTime.Now,
            isPaidFor = false,
            InvoiceItems =
            [
                new() {
                    Article = new() {
                        Name = "Artikel 1"
                    },
                    TotalPrice = 7.00f,
                    Amount = 2
                },
                new() {
                    Article = new() {
                        Name = "Artikel 2"
                    },
                    TotalPrice = 3.50f,
                    Amount = 1
                }
            ],
            Person = new() { FirstName = "Max", LastName = "Mustermann" }
        };
        
        AllArticles =
        [
            new Article { Id = 1, Name = "Bier", PriceMember = 3.00f, Type = ArticleType.FOOD },
            new Article { Id = 2, Name = "Wasser", PriceMember = 1.50f, Type = ArticleType.DRINK },
            new Article { Id = 3, Name = "Cola", PriceMember = 2.00f, Type = ArticleType.DRINK },
            new Article { Id = 4, Name = "Pizza", PriceMember = 8.00f, Type = ArticleType.FOOD },
            new Article { Id = 5, Name = "Burger", PriceMember = 6.50f, Type = ArticleType.FOOD }
        ];

        InitAsync();

        AddArticleCommand = new Command<Article>(AddArticleToInvoice);
        FilterArticlesCommand = new Command<ArticleType>(FilterArticlesByType);
        PayInvoiceCommand = new Command(PayInvoice);


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
                InvoiceItems = new List<InvoiceItem>()
            };
        }
    }

    private void AddArticleToInvoice(Article article)
    {
        // To add articles to an invoice, first check if the article is alreay contained in the invoice.
        InvoiceItem ExistingInvoiceItem = _Invoice.InvoiceItems
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
            _Invoice.InvoiceItems.Add(newItem);
            OnPropertyChanged(nameof(Invoice));
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

    public void PayInvoice()
    {
        _Invoice.isPaidFor = true;

        CloseView();
    }

    public void CloseView()
    {
        _invoiceDb.SaveInvoiceAsync(_Invoice);

        Shell.Current.GoToAsync(nameof(HomePage));
    }
}