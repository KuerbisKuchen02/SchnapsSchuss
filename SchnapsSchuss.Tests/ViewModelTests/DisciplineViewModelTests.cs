using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;
using SchnapsSchuss.Tests.ViewModels;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Tests.ViewModels
{
    public class DisciplineViewModelTests
    {
        [Fact]
        public async Task AddDiscipline_ShouldAddDiscipline()
        {
            // ARRANGE
            ArticleDatabase db = new ArticleDatabase(new List<Article>
            {
                new Article { Id = 1, Stock = 5, PriceGuest = 10, PriceMember = 8, Name = "Existing Article", Type = ArticleType.DISCIPLINE }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Artikel" },
                { "shownColumns", Article.Columns },
                { "articleType", ArticleType.DISCIPLINE } // ensure type is set
            };
            var viewModel = new CrudViewModel<Article>(query, db);

            // ACT
            var newArticle = new Article
            {
                Id = 2,
                Name = "New Article",
                Stock = 10,
                PriceGuest = 15,
                PriceMember = 12
            };
            viewModel.SelectedItem = newArticle;
            viewModel.OnPopupSubmit();

            // ASSERT
            var allArticles = await db.GetAllAsync();
            Assert.Contains(allArticles, a => a.Id == 2 && a.Name == "New Article");
            Assert.Contains(viewModel.Items, a => a.Id == 2 && a.Name == "New Article");
        }

        [Fact]
        public async Task DeleteDiscipline_ShouldDeleteDiscipline()
        {
            // ARRANGE
            var db = new ArticleDatabase(new List<Article>
            {
                new Article { Id = 1, Stock = 5, PriceGuest = 10, PriceMember = 8, Name = "Existing Article", Type = ArticleType.DISCIPLINE },
                new Article { Id = 2, Stock = 10, PriceGuest = 15, PriceMember = 12, Name = "New Article", Type = ArticleType.DISCIPLINE }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Artikel" },
                { "shownColumns", Article.Columns },
                { "articleType", ArticleType.DISCIPLINE }
            };

            var viewModel = new CrudViewModel<Article>(query, db);

            // ACT
            var articleToDelete = db.GetAllAsync().Result.First(a => a.Id == 2);
            viewModel.SelectedItem = articleToDelete;
            viewModel.OnDelete();

            // ASSERT
            var allArticles = await db.GetAllAsync();
            Assert.DoesNotContain(allArticles, a => a.Id == 2);
            Assert.DoesNotContain(viewModel.Items, a => a.Id == 2);
            // Ensure existing article is still there
            Assert.Contains(allArticles, a => a.Id == 1);
        }

        [Fact]
        public async Task LoadItems_ShouldLoadAndFilterArticlesByType()
        {
            // ARRANGE
            var db = new ArticleDatabase(new List<Article>
            {
                new Article { Id = 1, Name = "Discipline 1", Type = ArticleType.DISCIPLINE },
                new Article { Id = 3, Name = "Discipline 2", Type = ArticleType.DISCIPLINE }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Artikel" },
                { "shownColumns", Article.Columns },
                { "articleType", ArticleType.DISCIPLINE } // filter by DISCIPLINE
            };

            var viewModel = new CrudViewModel<Article>(query, db);

            // ACT
            viewModel.LoadItems();

            // ASSERT
            // Only articles of type DISCIPLINE should be loaded
            Assert.All(viewModel.Items, a => Assert.Equal(ArticleType.DISCIPLINE, a.Type));
            Assert.Equal(2, viewModel.Items.Count); // two discipline articles
            Assert.Equal(viewModel.Items.Count, viewModel.FilteredItems.Count);

            // Check that specific articles are included
            Assert.Contains(viewModel.Items, a => a.Id == 1 && a.Name == "Discipline 1");
            Assert.Contains(viewModel.Items, a => a.Id == 3 && a.Name == "Discipline 2");
        }

        [Fact]
        public async Task UpdateDisciplineInInvoice_ShouldUpdateArticle()
        {
            // ARRANGE
            var db = new ArticleDatabase(new List<Article>
            {
                new Article { Id = 1, Name = "Existing Article", Stock = 5, PriceGuest = 10, PriceMember = 8, Type = ArticleType.DISCIPLINE }
            });

            var query = new Dictionary<string, object>
            {
                { "title", "Artikel" },
                { "shownColumns", Article.Columns },
                { "articleType", ArticleType.DISCIPLINE }
            };

            var viewModel = new CrudViewModel<Article>(query, db);

            // ACT
            // Select the article to update
            var articleToUpdate = db.GetAllAsync().Result.First(a => a.Id == 1);
            viewModel.SelectedItem = articleToUpdate;

            // Modify some properties
            articleToUpdate.Name = "Updated Article";
            articleToUpdate.Stock = 20;
            articleToUpdate.PriceGuest = 12;

            // Submit changes
            viewModel.OnPopupSubmit();

            // ASSERT
            var updatedArticles = await db.GetAllAsync();
            var updatedArticle = updatedArticles.First(a => a.Id == 1);

            Assert.Equal("Updated Article", updatedArticle.Name);
            Assert.Equal(20, updatedArticle.Stock);
            Assert.Equal(12, updatedArticle.PriceGuest);

            // Also verify it's updated in the view model
            var vmArticle = viewModel.Items.First(a => a.Id == 1);
            Assert.Equal("Updated Article", vmArticle.Name);
            Assert.Equal(20, vmArticle.Stock);
            Assert.Equal(12, vmArticle.PriceGuest);
        }
    }
}
