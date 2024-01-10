using IfRolesExample.Data;
using IfRolesExample.Repositories;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IfRolesExample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRoleController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            UserRepo userRepo = new UserRepo(_db);
            IEnumerable<UserVM> users = userRepo.GetAllUsers();

            return View(users);
        }

        public async Task<IActionResult> Detail(string userName, string message = "")
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager, _db);
            var roles = await userRoleRepo.GetUserRolesAsync(userName);

            ViewBag.Message = message;
            ViewBag.UserName = userName;
            ViewBag.FullName = await userRoleRepo.GetUserFullNameAsync(userName);

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
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager, _db);

            if (ModelState.IsValid)
            {
                var result = await userRoleRepo.CreateUserRoleAsync(userRoleVM.Email, userRoleVM.RoleName);

                if (result.Success)
                {
                    string message = $"{userRoleVM.RoleName} permissions successfully added to {userRoleVM.Email}.";
                    return RedirectToAction("Detail", "UserRole", new { userName = userRoleVM.Email, message = message });
                }

                ModelState.AddModelError("", result.ErrorMessage);
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
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager, _db);
            var result = await userRoleRepo.RemoveUserRoleAsync(userRoleVM.Email, userRoleVM.RoleName);

            var roles = await userRoleRepo.GetUserRolesAsync(userRoleVM.Email);

            ViewBag.Message = result.Success ? "UserRole removed successfully." : result.ErrorMessage;
            ViewBag.UserName = userRoleVM.Email;

            return View("Detail", roles);
        }
    }
}

