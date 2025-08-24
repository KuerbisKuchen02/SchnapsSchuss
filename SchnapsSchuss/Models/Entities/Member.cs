using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public class Member
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [ForeignKey(typeof(Person))]
    public int PersonId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set;} = string.Empty;
    public bool IsRangeSupervisor { get; set; }
    
    [OneToOne(CascadeOperations = CascadeOperation.All)]
    public Person Person { get; set; } = new();

    [Ignore]
    public string FirstName
    {
        get => Person.FirstName; 
        set => Person.FirstName = value;
    }

    [Ignore]
    public string LastName
    {
        get => Person.LastName;
        set => Person.LastName = value;
    }

    [Ignore]
    public DateTime DateOfBirth
    {
        get => Person.DateOfBirth;
        set => Person.DateOfBirth = value;
    }

    [Ignore]
    public bool OwnsGunOwnershipCard
    {
        get => Person.OwnsGunOwnershipCard;
        set => Person.OwnsGunOwnershipCard =  value;
    }

    [Ignore]
    public RoleType Role
    {
        get => Person.Role;
        set => Person.Role = value;
    }
    
    public static readonly Dictionary<string, string> Columns = new()
    {
        {nameof(Username), "Benutzername"},
        {nameof(Password), "Passwort"},
        {nameof(FirstName), "Vorname"},
        {nameof(LastName), "Nachname"},
        {nameof(Entities.Person.Role), "Rolle"},
        {nameof(IsRangeSupervisor), "Ist Standaufsicht"},
        {nameof(OwnsGunOwnershipCard), "Hat Waffenkarte"},
    };
}