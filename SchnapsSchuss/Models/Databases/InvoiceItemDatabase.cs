using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class InvoiceItemDatabase : Database<InvoiceItem>
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
        await Init();
        if (invoice is null) throw new ArgumentNullException(nameof(invoice));
        
        return await database.Table<InvoiceItem>()
            .Where(i => i.InvoiceId == invoice.Id)
            .ToListAsync();
    }

    public async Task<InvoiceItem> GetOneAsync(int id)
    {
        await Init();
        return await database.Table<InvoiceItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<InvoiceItem>> GetAllAsync()
    {
        await Init();
        return await database.Table<InvoiceItem>().ToListAsync();
    }

    public async Task<int> SaveAsync(InvoiceItem entity)
    {
        await Init();
        if (entity.Id != 0)
            return await database.UpdateAsync(entity);
        else
            return await database.InsertAsync(entity);
    }

    public async Task<int> SaveInvoiceItemsAsync(List<InvoiceItem> entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (entity.Count == 0) return 0;

        await Init();

        int affectedRows = 0;

        await database.RunInTransactionAsync(tran =>
        {
            foreach (var item in entity)
            {
                if (item.Id != 0)
                    affectedRows += tran.Update(item);
                else
                    affectedRows += tran.Insert(item);
            }
        });

        return affectedRows;
    }

    public async Task<int> DeleteAsync(InvoiceItem entity)
    {
        await Init();
        return await database.DeleteAsync(entity);
    }
}