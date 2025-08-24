using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.Tests.Models.Databases
{
    internal class MemberDatabase : Database<Member>
    {
        private readonly List<Member> _members;

        public MemberDatabase(List<Member> members = null)
        {
            _members = members ?? new List<Member>();

            // Ensure default admin exists
            if (!_members.Any(m => m.Username == "admin"))
            {
                var admin = new Member
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin",
                    Role = RoleType.ADMINISTRATOR,
                    Person = new Person { Id = 1, Role = RoleType.ADMINISTRATOR }
                };
                _members.Add(admin);
            }
        }

        public Task<int> DeleteAsync(Member entity)
        {
            _members.Remove(entity);
            return Task.FromResult(1);
        }

        public Task<List<Member>> GetAllAsync()
        {
            return Task.FromResult(_members.ToList());
        }

        public Task<Member> GetOneAsync(int id)
        {
            var member = _members.FirstOrDefault(m => m.PersonId == id || m.Id == id);
            return Task.FromResult(member);
        }

        public Task<Member?> CheckIfUserExists(string username, string password)
        {
            var member = _members.FirstOrDefault(m => m.Username == username && m.Password == password);
            return Task.FromResult(member);
        }

        public Task<int> SaveAsync(Member entity)
        {
            var existing = _members.FirstOrDefault(m => m.Id == entity.Id);
            if (existing != null)
            {
                _members.Remove(existing); // replace existing
            }
            else
            {
                // Assign a new Id if 0
                if (entity.Id == 0) entity.Id = _members.Count > 0 ? _members.Max(m => m.Id) + 1 : 1;
                if (entity.Person.Id == 0) entity.Person.Id = entity.Id;
            }

            _members.Add(entity);
            return Task.FromResult(1);
        }
    }
}