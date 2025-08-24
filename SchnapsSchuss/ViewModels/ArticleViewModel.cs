using CommunityToolkit.Mvvm.ComponentModel;
using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.ViewModels
{
    public class ArticleViewModel : ObservableObject
    {
        // article and invoice are necessary because the
        // price is dependent on the person inside the invoice
        public Article Article { get; }
        private readonly Invoice _invoice;

        public ArticleViewModel(Article article, Invoice invoice)
        {
            Article = article;
            _invoice = invoice;

            // Subscribe to changes inside the article
            Article.PropertyChanged += (_, e) =>
            {
                // Only stock changes inside the article are relevant for the UI
                if (e.PropertyName == nameof(Article.Stock))
                    OnPropertyChanged(nameof(IsInStock));  // Update isInStock which is bound to the UI
            };
        }

        /*
         * Determines the correct price and returns the article name and price as a single String
         */
        public string DisplayNameWithPrice => $"{Article.Name} - {(_invoice.Person.Role == RoleType.GUEST ? Article.PriceGuest : Article.PriceMember):0.00}€";
            

        /*
         * Returns a boolean representing if the Article is currently in stock.
         */
        public bool IsInStock => Article.Stock is > 0 or -1;
    }
}
