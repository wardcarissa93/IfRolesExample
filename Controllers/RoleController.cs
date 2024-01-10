using IfRolesExample.Data;
using IfRolesExample.Repositories;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Core;

namespace IfRolesExample.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            this._db = db;
        }

        public IActionResult Index()
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            var roles = roleRepo.GetAllRoles();

            return View(roles);
        }

        [HttpGet]
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
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Role creation failed." +
                                             " The role may already exist.");
                }
            }
            return View(roleVM);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            RoleRepo roleRepo = new RoleRepo(_db);

            var role = roleRepo.GetRole(id);

            if (role == null)
            {
                // Handle the case where the role with the provided id is not found
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        [HttpPost]
        public IActionResult Delete(RoleVM role)
        {
            RoleRepo roleRepo = new RoleRepo(_db);

            // Check if the role is assigned to any user
            if (roleRepo.IsRoleAssigned(role.Id))
            {
                TempData["ErrorMessage"] = "Role cannot be deleted because it is assigned to a user.";
                return RedirectToAction(nameof(Index));
            }

            // If the role is not assigned, proceed with deletion
            bool isSuccess = roleRepo.DeleteRole(role.Id);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "Role deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Role deletion failed.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
