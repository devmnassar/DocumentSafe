using System.Security.Claims;
using DocSafe.DataAccess.Repository.IRepository;
using DocSafe.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


namespace DocSafe.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DocumentManageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentManageController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var documentList = _unitOfWork.Documents.GetAll(includeProperty: "ApplicationUser");
            return View(documentList);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Get Document By ID, And make Check 

            var document = _unitOfWork.Documents.GetById(u => u.Id == id, includeProperty: "ApplicationUser");
            if (document == null) return NotFound();

            // Generate PDF preview URL
            string pdfUrl = Url.Content("~/uploads/" + System.IO.Path.GetFileName(document.FilePath));

            //Passing Url To View Bag
            ViewBag.PdfUrl = pdfUrl;

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            // Get Document By ID, And make Check 
            var documentToBeDeleted = _unitOfWork.Documents.GetById(u => u.Id == id, includeProperty: "ApplicationUser");
            if (documentToBeDeleted == null) return NotFound();

            // Delete Document From Folder 
            // Getting A DocumentPath
            var filePath = documentToBeDeleted.FilePath;
            var oldDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.Trim('\\'));

            // Check If Document Exists
            if (System.IO.File.Exists(oldDocumentPath))
            {
                // Deleting File
                System.IO.File.Delete(oldDocumentPath);
            }

            // Removing From Database
            _unitOfWork.Documents.Remove(documentToBeDeleted);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Document Deleted";
            return RedirectToAction("Index");
        }

        [HttpGet]
        // Download Function For PDF Files
        public IActionResult Download(int id)
        {
            // Get Document By ID, And make Check 
            var document = _unitOfWork.Documents.GetById(u => u.Id == id, includeProperty: "ApplicationUser");
            if (document == null)
            {
                TempData["error"] = "Document Not Found";
                return RedirectToAction("Index");
            }

            //Download Function
            var filePath = document.FilePath;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            TempData["success"] = "Document Downloaded";
            return File(fileBytes, "application/pdf", System.IO.Path.GetFileName(filePath));
        }

        #region API CALLS
        // Loading DataTable
        [HttpGet]
        public IActionResult GetAll()
        {
            // Getting All Documents Of User
            List<Document> documentList = _unitOfWork.Documents.GetAll(includeProperty: "ApplicationUser").ToList();
            return Json(new { data = documentList });
        }

        [HttpGet]
        public IActionResult Search(string query)
        {

            //Searching Through The Text In PDF Files
            var results = _unitOfWork.Documents.Search(query);

            return Json(new { data = results });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // Get Document By ID, And make Check 
            var documentToBeDeleted = _unitOfWork.Documents.GetById(u => u.Id == id, includeProperty: "ApplicationUser");
            if (documentToBeDeleted == null) return NotFound();

            // Delete Document From Folder 
            // Getting A DocumentPath
            var filePath = documentToBeDeleted.FilePath;
            var oldDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.Trim('\\'));

            // Check If Document Exists
            if (System.IO.File.Exists(oldDocumentPath))
            {
                // Deleting File
                System.IO.File.Delete(oldDocumentPath);
            }

            // Removing From Database
            _unitOfWork.Documents.Remove(documentToBeDeleted);
            _unitOfWork.SaveChanges();
            TempData["success"] = "Document Deleted";
            //return RedirectToAction("Index");
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
