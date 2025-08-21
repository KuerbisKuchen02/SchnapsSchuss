using System.Collections.ObjectModel;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.ViewModels;

public class CashRegisterViewModel : BaseViewModel
{
    private InvoiceDatabase _invoiceDb;
    private ArticleDatabase _articleDb;
    
    // All Articles as an Observable Collection
    public ObservableCollection<Article> Articles { get; } = new ObservableCollection<Article>();
    
    // Current Invoice
    private Invoice _invoice;

    public Invoice Invoice
    {
        get => _invoice;
        set =>  SetProperty(ref _invoice, value);
    }
    
    public CashRegisterViewModel(Person person)
    {
        _articleDb = new ArticleDatabase();
        _invoiceDb = new InvoiceDatabase();
        
        // TODO: Load Invoice here
        InitAsync();
    }

    private async Task InitAsync()
    {
        await LoadArticlesAsync();
    }

    private async Task LoadArticlesAsync()
    {
        var articles = await _articleDb.GetArticlesAsync();
        foreach (var article in articles)
            Articles.Add(article);
    }

    private void addArticleToInvoice(Article article)
    {
        InvoiceItem invoiceItem = new InvoiceItem();
        
        _invoice.invoiceItems.Add();
    }
}