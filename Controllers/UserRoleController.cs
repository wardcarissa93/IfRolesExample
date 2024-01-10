using IfRolesExample.Data;
using IfRolesExample.Repositories;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IfRolesExample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRoleController(ApplicationDbContext context,
                                 UserManager<IdentityUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            UserRepo userRepo = new UserRepo(_db);
            IEnumerable<UserVM> users = userRepo.GetAllUsers();

            return View(users);
        }

        public async Task<IActionResult> Detail(string userName,
                                                string message = "")
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager);


            var roles = await userRoleRepo.GetUserRolesAsync(userName);

            ViewBag.Message = message;
            ViewBag.UserName = userName;

            return View(roles);
        }

        public ActionResult Create()
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            ViewBag.RoleSelectList = roleRepo.GetRoleSelectList();


            UserRepo userRepo = new UserRepo(_db);
            ViewBag.UserSelectList = userRepo.GetUserSelectList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRoleVM userRoleVM)
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager);

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the user already has the specified role
                    var user = await _userManager.FindByEmailAsync(userRoleVM.Email);
                    var hasRole = await _userManager.IsInRoleAsync(user, userRoleVM.RoleName);

                    if (hasRole)
                    {
                        ModelState.AddModelError("", $"User {userRoleVM.Email} already has the role {userRoleVM.RoleName}.");
                    }
                    else
                    {
                        var addUR = await userRoleRepo.AddUserRoleAsync(userRoleVM.Email, userRoleVM.RoleName);

                        string message = $"{userRoleVM.RoleName} permissions" +
                                        $" successfully added to " +
                                        $"{userRoleVM.Email}.";

                        return RedirectToAction("Detail", "UserRole",
                            new
                            {
                                userName = userRoleVM.Email,
                                message = message
                            });
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "UserRole creation failed.");
                    ModelState.AddModelError("", "The Role may exist for this user.");
                }
            }

            RoleRepo roleRepo = new RoleRepo(_db);
            ViewBag.RoleSelectList = roleRepo.GetRoleSelectList();

            UserRepo userRepo = new UserRepo(_db);
            ViewBag.UserSelectList = userRepo.GetUserSelectList();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(UserRoleVM userRoleVM)
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager);

            try
            {
                bool isSuccess = await userRoleRepo.RemoveUserRoleAsync(userRoleVM.Email, userRoleVM.RoleName);

                if (isSuccess)
                {
                    ViewBag.DeleteSuccessMessage = $"{userRoleVM.RoleName} permissions successfully removed from {userRoleVM.Email}.";
                }
                else
                {
                    ViewBag.DeleteErrorMessage = $"Failed to remove {userRoleVM.RoleName} permissions from {userRoleVM.Email}.";
                }
            }
            catch (Exception)
            {
                ViewBag.DeleteErrorMessage = "An error occurred while removing UserRole.";
            }

            var roles = await userRoleRepo.GetUserRolesAsync(userRoleVM.Email);

            ViewBag.Message = ViewBag.DeleteSuccessMessage ?? ViewBag.DeleteErrorMessage;
            ViewBag.UserName = userRoleVM.Email;

            return View("Detail", roles);
        }
    }
}

