using DocSafe.DataAccess.Repository.IRepository;
using DocSafe.Model.Models;
using DocSafe.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DocSafe.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class UsersManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersManagementController(IUnitOfWork unitOfWork , UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            UserManageVM userManageVM = new UserManageVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUsers.GetById(x => x.Id == id),
                RoleList = _roleManager.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                })
            };

            userManageVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUsers.GetById(u => u.Id == id))
                    .GetAwaiter().GetResult().FirstOrDefault();

            return View(userManageVM);

        }
        [HttpPost("Edit")]
        public IActionResult Edit(UserManageVM userManageVM)
        {

            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUsers.GetById(u => u.Id == userManageVM.ApplicationUser.Id))
                    .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUsers.GetById(x => x.Id == userManageVM.ApplicationUser.Id);

            // Checking If User-Updated-Role
            if (!(userManageVM.ApplicationUser.Role == oldRole))
            {

                _unitOfWork.ApplicationUsers.Update(applicationUser);
                _unitOfWork.SaveChanges();

                // Removing Old Role
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();

                // Adding New Role
                _userManager.AddToRoleAsync(applicationUser, userManageVM.ApplicationUser.Role).GetAwaiter().GetResult();

                return RedirectToAction("Index");
            }
            else {

                _unitOfWork.ApplicationUsers.Update(applicationUser);
                _unitOfWork.SaveChanges();

                return RedirectToAction("Index");

            }

        }

        #region API CALLS
        // Loading DataTable
        [HttpGet]
        public IActionResult GetAll()
        {

            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUsers.GetAll().ToList();

            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            }

            return Json(new { data = objUserList });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            // Getting User
            var applicationUser = _unitOfWork.ApplicationUsers.GetById(u => u.Id == id);
            // Remove From Database
            _unitOfWork.ApplicationUsers.Remove(applicationUser);
            // SaveChanges
            _unitOfWork.SaveChanges();

            TempData["success"] = "Document Deleted";

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
