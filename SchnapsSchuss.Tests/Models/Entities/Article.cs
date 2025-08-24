namespace SchnapsSchuss.Tests.Models.Entities
{
    public partial class Article
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float PriceMember { get; set; }
        public float PriceGuest { get; set; }
        public ArticleType Type { get; set; }
        public int Stock { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
        public static readonly Dictionary<string, string> Columns = new()
    {
        {nameof(Name), "Name"},
        {nameof(PriceMember), "Preis für Mitglieder"},
        {nameof(PriceGuest), "Preis für Gäste"},
        {nameof(Stock), "Bestand"},
    };
    }

}
