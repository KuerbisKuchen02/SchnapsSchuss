using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public class Invoice
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool isPaidFor { get; set;  }
    
    [ForeignKey(typeof(Person))]
    public int PersonId { get; set; }
    
    [ManyToOne]
    public Person Person { get; set; }
    
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<InvoiceItem> invoiceItems { get; set; }
}