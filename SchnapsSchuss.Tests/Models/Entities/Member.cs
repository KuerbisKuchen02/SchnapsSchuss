namespace SchnapsSchuss.Tests.Models.Entities
{
    public class Member
    {
        public int PersonId { get; set; }
        public string Username { get; set; }
        public string Password { get; set;}
        public bool IsRangeSupervisor { get; set; }
        public Person person { get; set; }
        public static readonly List<string> Columns = ["Username", "Password", "IsRangeSupervisor"];
    }
}
