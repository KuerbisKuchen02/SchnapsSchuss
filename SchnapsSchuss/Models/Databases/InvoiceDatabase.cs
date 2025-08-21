using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class InvoiceDatabase
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
                TotalPrice REAL NOT NULL,
                Amount INTEGER NOT NULL,
                FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id) ON DELETE CASCADE
            );
        ");
    }

    public async Task<List<Invoice>> GetInvoicesAsync()
    {
        await Init();
        return await database.Table<Invoice>().ToListAsync();
    }

    public async Task<Invoice> GetInvoiceAsync(int id)
    {
        await Init();
        return await database.Table<Invoice>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveInvoiceAsync(Invoice invoice)
    {
        await Init();
        if (invoice.Id != 0)
            return await database.UpdateAsync(invoice);
        else
            return await database.InsertAsync(invoice);
    }

    public async Task<int> DeleteInvoiceAsync(Invoice invoice)
    {
        await Init();
        return await database.DeleteAsync(invoice);
    }
}