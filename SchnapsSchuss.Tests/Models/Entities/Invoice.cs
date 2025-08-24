namespace SchnapsSchuss.Tests.Models.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool isPaidFor { get; set;  }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
    }
}
