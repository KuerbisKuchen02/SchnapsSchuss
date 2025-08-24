using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.Tests.Models.Databases
{
    internal class ArticleDatabase : Database<Article>
    {
        private readonly List<Article> _articles;

        public ArticleDatabase(List<Article> articles = null)
        {
            _articles = articles ?? new List<Article>();
        }

        public Task<int> DeleteAsync(Article entity)
        {
            _articles.Remove(entity);
            return Task.FromResult(1); // 1 = deleted
        }

        public Task<List<Article>> GetAllAsync()
        {
            return Task.FromResult(_articles.ToList());
        }

        public Task<Article> GetOneAsync(int id)
        {
            var article = _articles.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(article);
        }

        public Task<int> SaveAsync(Article entity)
        {
            var existing = _articles.FirstOrDefault(a => a.Id == entity.Id);
            if (existing != null)
            {
                _articles.Remove(existing); // replace existing
            }
            _articles.Add(entity);
            return Task.FromResult(1); // 1 = saved
        }

        public Task<List<Article>> GetArticlesOfTypeAsync(ArticleType articleType)
        {
            var filtered = _articles.Where(a => a.Type == articleType).ToList();
            return Task.FromResult(filtered);
        }
    }
}