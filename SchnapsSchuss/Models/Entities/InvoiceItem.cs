using SQLite;
using SQLiteNetExtensions.Attributes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SchnapsSchuss.Models.Entities;

public partial class InvoiceItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [ForeignKey(typeof(Invoice))]
    public int InvoiceId { get; set; }

    [ForeignKey(typeof(Article))]
    public int ArticleId { get; set; }

    [ObservableProperty]
    private float totalPrice;

    [ObservableProperty]
    private int amount;

    [ManyToOne]
    public Invoice Invoice { get; set; }
    
    [ManyToOne]
    public Article Article { get; set; }
}
