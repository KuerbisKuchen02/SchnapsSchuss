using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Entities;
using SchnapsSchuss.Tests.ViewModels;

namespace SchnapsSchuss.Tests.ViewModels
{
    public class CashRegisterViewModelTests
    {
        /*
         * Tests if adding an article which does not exist in an invoice adds it correctly.
         */
        [Fact]
        public void AddArticleToInvoice_ShouldAddInvoiceItem()
        {
            // ARRANGE
            Article Article = new Article { Id = 1, Stock = 5, PriceGuest = 10, PriceMember = 8 };
            Person Person = new Person { Id = 1, Role = RoleType.GUEST };
            Invoice Invoice = new Invoice { Person = Person, PersonId = Person.Id, InvoiceItems = new List<InvoiceItem>() };

            var viewModel = new CashRegisterViewModel();

            viewModel.Invoice = Invoice;

            // ACT
            viewModel.AddArticleToInvoice(Article);

            // ASSERT
            Assert.Single(viewModel.InvoiceItems);
            Assert.Equal(Article.Id, viewModel.InvoiceItems.First().Article.Id);
            Assert.Equal(10, viewModel.InvoiceItems.First().TotalPrice);
        }

        /*
         * Tests if adding an article which already exists in the invoice inreases the amount.
         */
        [Fact]
        public void AddArticleToInvoice_ShouldIncreaseInvoiceItem()
        {
            // ARRANGE
            Article article = new Article { Id = 1, Stock = 5, PriceGuest = 10, PriceMember = 8 };
            Person person = new Person { Id = 1, Role = RoleType.GUEST };

            InvoiceItem invoiceItem = new InvoiceItem { Article = article, ArticleId = article.Id, Amount = 1, TotalPrice = article.PriceGuest };
            Invoice invoice = new Invoice
            {
                Person = person,
                PersonId = person.Id,
                InvoiceItems = new List<InvoiceItem>()
            };

            var viewModel = new CashRegisterViewModel
            {
                Invoice = invoice,
                InvoiceItems = new List<InvoiceItem>() { invoiceItem }
            };

            // ACT
            viewModel.AddArticleToInvoice(article);

            // ASSERT
            Assert.Single(viewModel.InvoiceItems); // Still only one invoice item
            Assert.Equal(2, viewModel.InvoiceItems[0].Amount); // Amount increased by 1
            Assert.Equal(20, invoiceItem.TotalPrice); // TotalPrice updated correctly (Amount * PriceGuest)
        }

        /*
         * Tests if trying to add an article to the invoice which has a stock = 0, does not add the article.
         */
        [Fact]
        public void AddArticleToInvoice_StockZero()
        {
            // ARRANGE
            Article article = new Article { Id = 1, Stock = 0, PriceGuest = 10, PriceMember = 8 };
            Person person = new Person { Id = 1, Role = RoleType.GUEST };
            Invoice invoice = new Invoice { Person = person, PersonId = person.Id, InvoiceItems = new List<InvoiceItem>() };

            var viewModel = new CashRegisterViewModel
            {
                Invoice = invoice,
            };

            // ACT
            viewModel.AddArticleToInvoice(article);

            // ASSERT
            Assert.Empty(viewModel.InvoiceItems); // Article has not been added.
        }
    }
}

    
