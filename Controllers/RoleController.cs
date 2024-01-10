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

                try
                {
                    bool isSuccess = roleRepo.CreateRole(roleVM.RoleName);

                    if (isSuccess)
                    {
                        ViewBag.Message = "Role created successfully.";
                        return RedirectToAction(nameof(Index), new { message = ViewBag.Message });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Role creation failed.");
                        ModelState.AddModelError("", "The role may already exist.");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Role creation failed.");
                    ModelState.AddModelError("", "An unexpected error occurred.");
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

            try
            {
                // Check if the role is assigned to any user
                if (roleRepo.IsRoleAssigned(roleVM.RoleName))
                {
                    ViewBag.DeleteErrorMessage = "Cannot delete role. It is assigned to a user.";
                    return RedirectToAction(nameof(Index), new { message = ViewBag.DeleteErrorMessage });
                }

                _ = roleRepo.DeleteRole(roleVM.RoleName);

                ViewBag.DeleteSuccessMessage = "Role deleted successfully.";

                return RedirectToAction(nameof(Index), new { message = ViewBag.DeleteSuccessMessage });
            }
            catch (Exception)
            {
                ViewBag.DeleteErrorMessage = "An error occurred while deleting the role.";
                return RedirectToAction(nameof(Index), new { message = ViewBag.DeleteErrorMessage });
            }
        }

    }
}
