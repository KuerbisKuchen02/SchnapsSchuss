using SchnapsSchuss.Models.Entities;
using SQLiteNetExtensionsAsync.Extensions;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class InvoiceDatabase : Database<Invoice>
{
    SQLiteAsyncConnection database;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        await database.ExecuteAsync("PRAGMA foreign_keys = ON;");
        await database.CreateTableAsync<Invoice>();

        // You need to manually create the table because CreateTableAsync does not include the foreign key constraint.
        await database.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS InvoiceItem (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceId INTEGER NOT NULL,
                ArticleId INTEGER NOT NULL,
                TotalPrice REAL NOT NULL,
                Amount INTEGER NOT NULL,
                FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id) ON DELETE CASCADE,
                FOREIGN KEY (ArticleId) REFERENCES Article(Id) ON DELETE RESTRICT
            );
        ");
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        await Init();

        var invoices = await database.GetAllWithChildrenAsync<Invoice>(recursive: true);

        var allItems = invoices.SelectMany(i => i.InvoiceItems).ToList();
        var ids = allItems.Select(it => it.ArticleId).Distinct().ToList();

        var articles = await database.Table<Article>()
                                     .Where(a => ids.Contains(a.Id))
                                     .ToListAsync();
        var map = articles.ToDictionary(a => a.Id);

        foreach (var it in allItems)
            if (map.TryGetValue(it.ArticleId, out var a)) it.Article = a;

        return invoices;
    }

    public async Task<Invoice> GetOneAsync(int id)
    {
        await Init();
        return await database.Table<Invoice>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Invoice invoice)
    {
        int returnValue = 0;

        await Init();
        if (invoice.Id != 0)
        {
            returnValue = await database.UpdateAsync(invoice);
        }
        else
        {
            returnValue = await database.InsertAsync(invoice);
        }

        InvoiceItemDatabase invoiceItemDatabase = new();

        foreach (var it in invoice.InvoiceItems)
            it.InvoiceId = invoice.Id;

        await invoiceItemDatabase.SaveInvoiceItemsAsync(invoice.InvoiceItems);

        return returnValue;
    }

    public async Task<Invoice> GetOpenInvoiceForPerson(int Id)
    {
        await Init();
        return await database.Table<Invoice>().Where(i => i.isPaidFor == false && i.PersonId == Id).FirstOrDefaultAsync();
    }

    public async Task<int> DeleteAsync(Invoice invoice)
    {
        await Init();
        return await database.DeleteAsync(invoice);
    }
}