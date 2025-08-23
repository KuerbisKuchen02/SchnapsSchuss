namespace SchnapsSchuss.Tests.Models.Entities
{
    public partial class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ArticleId { get; set; }
        public float TotalPrice;
        public int Amount;
        public Invoice Invoice { get; set; }
        public Article Article { get; set; }
    }
}
