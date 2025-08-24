using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class InvoiceItemDatabase : IDatabase<InvoiceItem>
{
    private readonly SQLiteAsyncConnection _database;

    public InvoiceItemDatabase()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        _database.CreateTableAsync<InvoiceItem>();
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsOfInvoiceAsync(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        return await _database.Table<InvoiceItem>()
            .Where(i => i.InvoiceId == invoice.Id)
            .ToListAsync();
    }

    public async Task<InvoiceItem> GetOneAsync(int id)
    {
        return await _database.Table<InvoiceItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<InvoiceItem>> GetAllAsync()
    {
        return await _database.Table<InvoiceItem>().ToListAsync();
    }

    public async Task<int> SaveAsync(InvoiceItem entity)
    {
        if (entity.Id != 0)
            return await _database.UpdateAsync(entity);
        return await _database.InsertAsync(entity);
    }

    public async Task<int> SaveInvoiceItemsAsync(List<InvoiceItem> entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.Count == 0) return 0;
        
        var affectedRows = 0;
        
        await _database.RunInTransactionAsync(tran =>
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
        return await _database.DeleteAsync(entity);
    }
}