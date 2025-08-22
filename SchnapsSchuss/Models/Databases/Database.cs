using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchnapsSchuss.Models.Databases
{
    interface Database<T>
    {
        T getOne(int id);

        IEnumerable<T> getAll();

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
