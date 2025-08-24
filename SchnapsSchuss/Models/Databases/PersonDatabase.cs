using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class PersonDatabase : Database<Person>
{
    private readonly SQLiteAsyncConnection _database;

    public PersonDatabase()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        _database.CreateTableAsync<Person>();
    }
    
    public async Task<List<Person>> GetAllAsync()
    {
        return await _database.Table<Person>().ToListAsync();
    }

    public async Task<Person> GetOneAsync(int id)
    {
        return await _database.Table<Person>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Person>> GetPersonToIdsAsync(List<int> ids)
    {
        return await _database.Table<Person>().Where(p => ids.Contains(p.Id)).ToListAsync();
    }
    
    public async Task<int> SaveAsync(Person person)
    {
        if (person.Id != 0)
        {
            await _database.UpdateAsync(person);
        }
        else
        {
            await _database.InsertAsync(person);
        }

        return person.Id;
    }
    
    public async Task<int> DeleteAsync(Person person)
    {
        return await _database.DeleteAsync(person);
    }
}