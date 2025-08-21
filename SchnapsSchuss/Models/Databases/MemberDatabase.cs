using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class MemberDatabase
{
    SQLiteAsyncConnection database;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        await database.ExecuteAsync("PRAGMA foreign_keys = ON;");
        await database.CreateTableAsync<Member>();

        // You need to manually create the table because CreateTableAsync does not include the foreign key constraint.
        await database.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Member (
                PersonId INTEGER PRIMARY KEY,
                Username TEXT NOT NULL,
                Password TEXT NOT NULL,
                IsRangeSupervisor BOOLEAN NOT NULL,
                FOREIGN KEY (PersonId) REFERENCES Person(Id) ON DELETE CASCADE
            );
        ");
    }

    public async Task<List<Member>> GetMembersAsync()
    {
        await Init();
        return await database.Table<Member>().ToListAsync();
    }

    public async Task<Member> GetMemberAsync(int Id)
    {
        await Init();
        return await database.Table<Member>().Where(m => m.PersonId == Id).FirstOrDefaultAsync();
    }

    public async Task<int> CheckIfUserExists(string Username, string Password)
    {
        await Init();
        return await database.Table<Member>().Where(m => m.Username == Username && m.Password == Password).CountAsync();
    }


    public async Task<int> SaveMemberAsync(Member member)
    {
        await Init();
        if (member.PersonId != 0)
            return await database.UpdateAsync(member);
        else
            return await database.InsertAsync(member);
    }

    public async Task<int> DeleteMemberAsync(Member member)
    {
        await Init();
        return await database.DeleteAsync(member);
    }
    
}