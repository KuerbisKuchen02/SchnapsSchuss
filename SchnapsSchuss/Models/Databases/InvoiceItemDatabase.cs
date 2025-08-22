using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class InvoiceItemDatabase
{
    SQLiteAsyncConnection database;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        await database.ExecuteAsync("PRAGMA foreign_keys = ON;");
        
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

        await database.CreateTableAsync<InvoiceItem>();
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsOfInvoiceAsync(Invoice invoice)
    {
        if (invoice is null) throw new ArgumentNullException(nameof(invoice));
        
        return await database.Table<InvoiceItem>()
            .Where(i => i.InvoiceId == invoice.Id)
            .ToListAsync();
    }

    public async Task<InvoiceItem> GetInvoiceItemAsync(int id)
    {
        await Init();
        return await database.Table<InvoiceItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveInvoiceItemAsync(InvoiceItem invoiceItem)
    {
        await Init();
        if (invoiceItem.Id != 0)
            return await database.UpdateAsync(invoiceItem);
        else
            return await database.InsertAsync(invoiceItem);
    }

    public async Task<int> SaveInvoiceItemsAsync(List<InvoiceItem> invoiceItems)
    {
        if (invoiceItems is null) throw new ArgumentNullException(nameof(invoiceItems));
        if (invoiceItems.Count == 0) return 0;

        await Init();

        int affectedRows = 0;

        await database.RunInTransactionAsync(tran =>
        {
            foreach (var item in invoiceItems)
            {
                if (item.Id != 0)
                    affectedRows += tran.Update(item);
                else
                    affectedRows += tran.Insert(item);
            }
        });

        return affectedRows;
    }

    public async Task<int> DeleteItemAsync(InvoiceItem invoiceItem)
    {
        await Init();
        return await database.DeleteAsync(invoiceItem);
    }
}