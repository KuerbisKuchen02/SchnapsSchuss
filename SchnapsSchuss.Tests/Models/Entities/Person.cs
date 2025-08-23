namespace SchnapsSchuss.Tests.Models.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool OwnsGunOwnershipCard { get; set; }
        public RoleType Role { get; set; }
        public List<Invoice> invoices { get; set; }
    }
}