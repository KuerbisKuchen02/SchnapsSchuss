using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public class Article
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public float PriceMember { get; set; }
    public float PriceGuest { get; set; }
    public int Stock { get; set; }
    public ArticleType Type { get; set; }
    
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<InvoiceItem> InvoiceItems { get; set; }
}