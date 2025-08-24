using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchnapsSchuss.Tests.Models.Databases;
using SchnapsSchuss.Tests.Models.Entities;

namespace SchnapsSchuss.Tests.Models.Databases
{
    public class PersonDatabase : Database<Person>
    {
        private readonly List<Person> _persons;

        public PersonDatabase(List<Person> persons = null)
        {
            _persons = persons ?? new List<Person>();
        }

        public Task<int> DeleteAsync(Person entity)
        {
            _persons.Remove(entity);
            return Task.FromResult(1);
        }

        public Task<List<Person>> GetAllAsync()
        {
            return Task.FromResult(_persons.ToList());
        }

        public Task<Person> GetOneAsync(int id)
        {
            var person = _persons.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(person);
        }

        public Task<int> SaveAsync(Person entity)
        {
            var existing = _persons.FirstOrDefault(p => p.Id == entity.Id);
            if (existing != null)
            {
                _persons.Remove(existing);
            }
            _persons.Add(entity);
            return Task.FromResult(1);
        }

        public Task<List<Person>> GetPersonToIdsAsync(IEnumerable<int> personIds)
        {
            if (personIds == null)
                return Task.FromResult(new List<Person>());

            var persons = _persons
                .Where(p => personIds.Contains(p.Id))
                .ToList();

            return Task.FromResult(persons);
        }
    }
}
