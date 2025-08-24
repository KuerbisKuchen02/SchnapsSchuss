using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchnapsSchuss.Models.Databases
{
    public interface Database<T>
    {
        Task<T> GetOneAsync(int id);

        Task<List<T>> GetAllAsync();

        Task<int> SaveAsync(T entity);

        Task<int> DeleteAsync(T entity);
    }
}
