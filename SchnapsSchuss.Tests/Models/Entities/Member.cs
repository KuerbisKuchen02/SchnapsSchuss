using System.ComponentModel.DataAnnotations.Schema;

namespace SchnapsSchuss.Tests.Models.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsRangeSupervisor { get; set; }
        public Person Person { get; set; } = new();

        public string FirstName
        {
            get => Person.FirstName;
            set => Person.FirstName = value;
        }

        public string LastName
        {
            get => Person.LastName;
            set => Person.LastName = value;
        }

        public DateTime DateOfBirth
        {
            get => Person.DateOfBirth;
            set => Person.DateOfBirth = value;
        }

        public bool OwnsGunOwnershipCard
        {
            get => Person.OwnsGunOwnershipCard;
            set => Person.OwnsGunOwnershipCard = value;
        }

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
}
