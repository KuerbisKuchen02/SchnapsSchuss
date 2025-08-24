using SchnapsSchuss.Models.Entities;
using SQLite;

namespace SchnapsSchuss.Models.Databases;

public class ArticleDatabase : Database<Article>
{
    private readonly SQLiteAsyncConnection _database;

    public ArticleDatabase()
    {
        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        _database.CreateTableAsync<Article>();
    }

    public async Task<List<Article>> GetAllAsync()
    {
        return await _database.Table<Article>().ToListAsync();
    }

    public async Task<Article> GetOneAsync(int id)
    {
        return await _database.Table<Article>().Where(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Article>> GetArticlesOfTypeAsync(ArticleType articleType)
    {
        return await _database.Table<Article>().Where(a => a.Type == articleType).ToListAsync();
    }

    public async Task<int> SaveAsync(Article article)
    {
        if (article.Id != 0) return await _database.UpdateAsync(article);
        return await _database.InsertAsync(article);
    }

    public async Task<int> DeleteAsync(Article article)
    { 
        return await _database.DeleteAsync(article);
    }
    
}