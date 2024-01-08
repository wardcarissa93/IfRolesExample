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
    }
}
