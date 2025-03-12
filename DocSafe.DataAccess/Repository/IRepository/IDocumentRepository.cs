using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocSafe.Model.Models;
using Microsoft.AspNetCore.Http;

namespace DocSafe.DataAccess.Repository.IRepository
{
    public interface IDocumentRepository : IRepository<Document>
    {
        bool SaveFile(Document doc, IFormFile file, string userId);
        IEnumerable<Document> Search(string query);
        string ExtractText(string filePath);
    }
}
