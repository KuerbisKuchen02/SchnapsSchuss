using SchnapsSchuss.Models.Entities;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SchnapsSchuss.Models.Databases;

public class MemberDatabase : Database<Member>
{
    SQLiteAsyncConnection database;
    PersonDatabase personDb;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        personDb = new PersonDatabase();

        await database.ExecuteAsync("PRAGMA foreign_keys = ON;");
        await database.CreateTableAsync<Member>();

        // You need to manually create the table because CreateTableAsync does not include the foreign key constraint.
        await database.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Member (
                Id INTEGER PRIMARY KEY,
                PersonId INTEGER NOT NULL,
                Username TEXT NOT NULL,
                Password TEXT NOT NULL,
                IsRangeSupervisor BOOLEAN NOT NULL,
                FOREIGN KEY (PersonId) REFERENCES Person(Id) ON DELETE CASCADE
            );
        ");

        // Check if the default member already exists
        var count = await database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Member;");
        if (count == 0)
        {
            // Insert default member
            await database.ExecuteAsync(@"
            INSERT INTO Member (PersonId, Username, Password, IsRangeSupervisor)
            VALUES (1, 'Admin', 'password', 1);
        ");
        }
    }

    public async Task<List<Member>> GetAllAsync()
    {
        await Init();
        return await database.GetAllWithChildrenAsync<Member>(recursive: true);
    }

    public async Task<Member> GetOneAsync(int Id)
    {
        await Init();
        return await database.Table<Member>().Where(m => m.PersonId == Id).FirstOrDefaultAsync();
    }

    public async Task<Member> CheckIfUserExists(string Username, string Password)
    {
        await Init();

        Member member = await database.Table<Member>().Where(m => m.Username == Username && m.Password == Password).FirstOrDefaultAsync();

        if (member == null)
        {
            return null;
        }
        else
        {
            member.Person = await database.Table<Person>().Where(p => p.Id == member.PersonId).FirstOrDefaultAsync();

            if (member.Person == null)
            {
                return null;
            }
            else
            {
                return member;
            }
        }
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