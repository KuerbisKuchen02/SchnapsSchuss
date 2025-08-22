using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public partial class Article : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public float PriceMember { get; set; }
    public float PriceGuest { get; set; }

    [ObservableProperty]
    private int _stock;
    public ArticleType Type { get; set; }
    
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<InvoiceItem> InvoiceItems { get; set; }

    public static readonly List<string> Columns = ["Name", "PriceMember", "PriceGuest", "Stock"];
}