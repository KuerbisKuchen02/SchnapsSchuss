using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.ViewModels
{
    public class ArticleViewModel : ObservableObject
    {
        // article and invoice are necessary because the
        // price is dependend on the person inside the invoice
        public Article Article { get; }
        private readonly Invoice _invoice;

        public ArticleViewModel(Article article, Invoice invoice)
        {
            Article = article;
            _invoice = invoice;

            // Subscribe to changes inside the article
            Article.PropertyChanged += (s, e) =>
            {
                // Only stock changes inside the article are relevant for the UI
                if (e.PropertyName == nameof(Article.Stock))
                    OnPropertyChanged(nameof(IsInStock));  // Update isInStock which is binded to the UI
            };
        }

        /*
         * Determines the correct price and returns the article name and price as a single String
         */
        public string DisplayNameWithPrice => $"{Article.Name} - {(_invoice.Person.Role == RoleType.GUEST ? Article.PriceGuest : Article.PriceMember):0.00}€";
            

        /*
         * Returns a boolean representing if the Aarticle is currently in stock.
         */
        public bool IsInStock => Article.Stock > 0 || Article.Stock == -1;
    }
}
