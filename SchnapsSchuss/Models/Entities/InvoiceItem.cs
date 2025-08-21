using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public class InvoiceItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [ForeignKey(typeof(Invoice))]
    public int InvoiceId { get; set; }
    public float TotalPrice { get; set; }
    public int Amount { get; set; }

    [ManyToOne]
    public Invoice Invoice { get; set; }
    
    [ManyToOne]
    public Article Article { get; set; }
}
