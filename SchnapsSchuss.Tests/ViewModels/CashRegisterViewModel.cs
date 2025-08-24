using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.Tests.ViewModels
{
    internal class CashRegisterViewModel
    {
        public List<InvoiceItem> InvoiceItems { get; set; }
        public List<Article> Articles { get; set; }
        public List<Article> FilteredArticles { get; set; }
        public Invoice Invoice { get; set; }

        public CashRegisterViewModel()
        {
            InvoiceItems = new List<InvoiceItem>();
            Articles = new List<Article>();
            FilteredArticles = new List<Article>();
        }

        public void AddArticleToInvoice(Article article)
        {
            // To add articles to an invoice, first check if the article is alreay contained in the invoice.
            InvoiceItem ExistingInvoiceItem = InvoiceItems
                .FirstOrDefault(i => i.Article.Id == article.Id);

            // Check if the article has stock left
            if (article.Stock <= 0)
                return;

            // Determine the correct price accroding to the role
            float articlePrice = Invoice.Person.Role == RoleType.GUEST ? article.PriceGuest : article.PriceMember;

            // Article already exists. Just increase the amount and re-calculate the price.
            if (ExistingInvoiceItem != null)
            {
                ExistingInvoiceItem.Amount++;
                ExistingInvoiceItem.TotalPrice = ExistingInvoiceItem.Amount * articlePrice;
                // OnPropertyChanged(nameof(ExistingInvoiceItem.Amount));
            }
            else
            // Article new, create a new InvoiceItem with the article.
            {
                InvoiceItem newItem = new()
                {
                    Article = article,
                    ArticleId = article.Id,
                    Amount = 1,
                    TotalPrice = articlePrice
                };
                InvoiceItems.Add(newItem);
            }

            // Update InvoiceTotal
            // OnPropertyChanged(nameof(InvoiceTotal));
            // subtractArticleAmount(article);
        }
    }
}
