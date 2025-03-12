using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocSafe.DataAccess.Data;
using DocSafe.DataAccess.Repository.IRepository;
using DocSafe.Model.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DocSafe.DataAccess.Repository
{
    public class DocumentRepository : Repository<Document> , IDocumentRepository
    {
        private readonly ApplicationDbContext _db;
        public DocumentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public string ExtractText(string filePath)
        {
            StringBuilder text = new StringBuilder();

            try
            {
                using (PdfReader reader = new PdfReader(filePath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF: {ex.Message}");
            }

            return text.ToString();
        }

        public bool SaveFile(Document doc, IFormFile file, string userId)
        {
            // file.pdf
            string fileName = System.IO.Path.GetFileName(file.FileName);

            // Upload-Folder
            string uploadsFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads");

            // Check If Directory Not Exist
            if (!System.IO.Path.Exists(uploadsFolder))
            {
                // Creates Folder
                Directory.CreateDirectory(uploadsFolder);
            }

            // New Generated File Name
            string newFileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);

            // localhost:1111/wwwroot/uploads/GUID.pdf
            string filePath = System.IO.Path.Combine(uploadsFolder, newFileName);

            using (var stream = new FileStream(System.IO.Path.Combine(uploadsFolder, newFileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string extractedText = ExtractText(filePath);

            var document = new Document
            {
                Name = fileName,
                Added = DateTime.Now,
                Type = doc.Type,
                Note = doc.Note,
                FilePath = filePath,
                ExtractedText = extractedText,
                ApplicationUserId = userId,
                GeneratedDocumentName = newFileName
            };

            _db.Add(document);
            _db.SaveChanges();
            //return filePath;
            return true;
        }

        public IEnumerable<Document> Search(string query)
        {
            return _db.Documents.Include(x=>x.ApplicationUser)
                          .Where(d => EF.Functions.Like(d.ExtractedText, $"%{query}%"))
                          .ToList();
        }
    }
}
