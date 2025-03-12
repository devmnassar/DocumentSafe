using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DocSafe.DataAccess.Data;
using DocSafe.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;

namespace DocSafe.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> _dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbset = _db.Set<T>();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperty = null)
        {
            IQueryable<T> query = _dbset;
            if (filter != null)
            {
                query = _dbset.Where(filter);
            }


            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var includeProp in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public T GetById(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracked = false)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = _dbset;
            }
            else
            {
                query = _dbset.AsNoTracking();
            }

            query = _dbset.Where(filter);

            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var includeProp in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }
    }
}
