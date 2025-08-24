using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class PersonDatabase : Database<Person>
{
    SQLiteAsyncConnection database;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await database.CreateTableAsync<Person>();
    }
    
    public async Task<List<Person>> GetAllAsync()
    {
        await Init();
        return await database.Table<Person>().ToListAsync();
    }

    public async Task<Person> GetOneAsync(int id)
    {
        await Init();
        return await database.Table<Person>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Person>> GetPersonToIdsAsync(List<int> Ids)
    {
        await Init();
        return await database.Table<Person>().Where(p => Ids.Contains(p.Id)).ToListAsync();
    }


    public async Task<List<Person>> GetPersonsOfRoleAsync(RoleType roleType)
    {
        await Init();
        return await database.Table<Person>().Where(p => p.Role == roleType).ToListAsync();
    }
    
    public async Task<int> SaveAsync(Person person)
    {
        await Init();
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
        await Init();
        return await database.DeleteAsync(person);
    }
    
}