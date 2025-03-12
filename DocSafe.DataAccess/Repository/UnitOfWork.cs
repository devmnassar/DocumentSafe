using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocSafe.DataAccess.Data;
using DocSafe.DataAccess.Repository.IRepository;

namespace DocSafe.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IDocumentRepository Documents { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Documents = new DocumentRepository(_db);
            ApplicationUsers = new ApplicationUserRepository(_db);
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
