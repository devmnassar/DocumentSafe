using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSafe.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IDocumentRepository Documents { get; }
        public IApplicationUserRepository ApplicationUsers { get; }

        void SaveChanges();
    }
}
