using IfRolesExample.Data;
using IfRolesExample.Repositories;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IfRolesExample.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        public ActionResult Index(string message = "")
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            ViewBag.Message = message;

            return View(roleRepo.GetAllRoles());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                RoleRepo roleRepo = new RoleRepo(_db);

                bool isSuccess = roleRepo.CreateRole(roleVM.RoleName);

                if (isSuccess)
                {
                    return RedirectToAction(nameof(Index), new { message = "Role deleted successfully." });
                }
                else
                {
                    ModelState.AddModelError("", "Role creation failed. The role may already exist.");
                }
            }

            return View(roleVM);
        }

        public ActionResult Delete(string roleName)
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            RoleVM roleVM = roleRepo.GetRole(roleName);

            if (roleVM == null)
            {
                return NotFound();
            }

            return View(roleVM);
        }

        [HttpPost]
        public ActionResult Delete(RoleVM roleVM)
        {
            RoleRepo roleRepo = new RoleRepo(_db);

            if (roleRepo.IsRoleAssigned(roleVM.RoleName))
            {
                ModelState.AddModelError("", "Cannot delete role. It is assigned to a user.");
                return View(roleVM); // Return the Delete view with the error message
            }

            bool isSuccess = roleRepo.DeleteRole(roleVM.RoleName);

            if (isSuccess)
            {
                return RedirectToAction(nameof(Index), new { message = "Role deleted successfully." });
            }
            else
            {
                ModelState.AddModelError("", $"An error occurred while deleting the role: Role not found.");
                return View(roleVM); // Return the Delete view with the error message
            }
        }
    }
}
