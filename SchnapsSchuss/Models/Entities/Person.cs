using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public class Person
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool OwnsGunOwnershipCard { get; set; }
    public RoleType Role { get; set; }
    
    [OneToOne(CascadeOperations = CascadeOperation.All)]
    public Member? member { get; set; } 
    
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<Invoice> invoices { get; set; }

    [Ignore]
    public Invoice OpenInvoice { get; set; }
}