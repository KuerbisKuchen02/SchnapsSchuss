using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class ArticleDatabase : Database<Article>
{
    SQLiteAsyncConnection database;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await database.CreateTableAsync<Article>();
    }
    
    public async Task<List<Article>> GetAllAsync()
    {
        await Init();
        return await database.Table<Article>().ToListAsync();
    }

    public async Task<Article> GetOneAsync(int id)
    {
        await Init();
        return await database.Table<Article>().Where(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Article>> GetArticlesOfTypeAsync(ArticleType articleType)
    {
        await Init();
        return await database.Table<Article>().Where(a => a.Type == articleType).ToListAsync();
    }

    public async Task<int> SaveAsync(Article article)
    {
        await Init();
        if (article.Id != 0)
            return await database.UpdateAsync(article);
        else
            return await database.InsertAsync(article);
    }

    public async Task<int> DeleteAsync(Article article)
    {
        await Init();
        return await database.DeleteAsync(article);
    }
    
}