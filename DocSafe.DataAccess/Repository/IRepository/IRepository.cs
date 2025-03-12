using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DocSafe.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T GetById(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracked = false);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperty = null);
        void Remove(T entity);
    }
}
