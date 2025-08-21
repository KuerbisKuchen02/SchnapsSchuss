using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SchnapsSchuss.Models.Entities;

public class Member
{
    [PrimaryKey, ForeignKey(typeof(Person))]
    public int PersonId { get; set; }
    public string Username { get; set; }
    public string Password { get; set;}
    public bool IsRangeSupervisor { get; set; }
    
    [OneToOne]
    public Person person { get; set; }
}