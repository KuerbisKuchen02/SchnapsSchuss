using SchnapsSchuss.Models.Entities;
using SQLiteNetExtensionsAsync.Extensions;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class InvoiceDatabase : IDatabase<Invoice>
{
    private readonly SQLiteAsyncConnection _database;
    private readonly InvoiceItemDatabase _invoiceItemDatabase;

    public InvoiceDatabase()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        _invoiceItemDatabase = new InvoiceItemDatabase();

        _database.ExecuteAsync("PRAGMA foreign_keys = ON;");
        _database.CreateTableAsync<Invoice>();
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        var invoices = await _database.GetAllWithChildrenAsync<Invoice>(recursive: true);

        var invoiceItems = invoices.SelectMany(i => i.InvoiceItems).ToList();
        var articleIds = invoiceItems.Select(it => it.ArticleId).Distinct().ToList();

        var articles = await _database.Table<Article>()
                                     .Where(a => articleIds.Contains(a.Id))
                                     .ToListAsync();
        
        var map = articles.ToDictionary(a => a.Id);

        foreach (var it in invoiceItems)
            if (map.TryGetValue(it.ArticleId, out var a)) it.Article = a;

        return invoices;
    }

    public async Task<Invoice> GetOneAsync(int id)
    {
        return await _database.Table<Invoice>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task<Invoice?> GetOpenInvoiceForPerson(int id)
    {
        return await _database.Table<Invoice>().Where(i => i.isPaidFor == false && i.PersonId == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Invoice invoice)
    {
        int returnValue;

        // If the invoice exists update it
        if (invoice.Id != 0)
        {
            returnValue = await _database.UpdateAsync(invoice);
        }
        // Else create a new one
        else
        {
            returnValue = await _database.InsertAsync(invoice);
        }

        InvoiceItemDatabase invoiceItemDatabase = new();

        foreach (var it in invoice.InvoiceItems)
            it.InvoiceId = invoice.Id;

        // Save invoice items
        await invoiceItemDatabase.SaveInvoiceItemsAsync(invoice.InvoiceItems);

        return returnValue;
    }

    public async Task<int> DeleteAsync(Invoice invoice)
    {
        return await _database.DeleteAsync(invoice);
    }
}