using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotnetCoding.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task Create(T entity);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        void Remove(T entity);
    }
}
