using SchnapsSchuss.Models.Entities;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SchnapsSchuss.Models.Databases;

public class MemberDatabase : Database<Member>
{
    private SQLiteAsyncConnection _database = null!;
    private PersonDatabase _personDatabase = null!;

    public MemberDatabase()
    {
        _ = Init();
    }

    private async Task Init()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        _personDatabase = new PersonDatabase();

        await _database.ExecuteAsync("PRAGMA foreign_keys = ON;");
        await _database.CreateTableAsync<Member>();
        
        // Check if the default member already exists
        var count = await _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Member;");
        if (count == 0)
        {
            await SaveAsync(new Member()
            {
                Username = "admin",
                Password = "admin",
                Role = RoleType.ADMINISTRATOR
            });
        }
    }

    public async Task<List<Member>> GetAllAsync()
    {
        await Init();
        return await database.GetAllWithChildrenAsync<Member>(recursive: true);
    }

    public async Task<Member> GetOneAsync(int id)
    {
        await Init();
        return await database.Table<Member>().Where(m => m.PersonId == Id).FirstOrDefaultAsync();
    }

    public async Task<Member?> CheckIfUserExists(string username, string password)
    {
        var member = await _database.Table<Member>().Where(m => m.Username == username && m.Password == password).FirstOrDefaultAsync();

        if (member == null) return null;
        
        member.Person = await _database.Table<Person>().Where(p => p.Id == member.PersonId).FirstOrDefaultAsync();
        
        return member.Person == null ? null : member;
    }
    
    public async Task<int> SaveAsync(Member member)
    {
        await Init();
        if (member.Id == 0) await database.InsertWithChildrenAsync(member, recursive: true);
        else await database.UpdateWithChildrenAsync(member);
        return 0;
    }

    public async Task<int> DeleteAsync(Member member)
    {
        await Init();
        await database.DeleteAsync(member, recursive: true);
        return 0;
    }
    
}