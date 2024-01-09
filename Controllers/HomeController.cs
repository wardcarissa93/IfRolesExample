using IfRolesExample.Data;
using IfRolesExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IfRolesExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,
                                ApplicationDbContext db)
        {
            this._logger = logger;
            this._db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult SecureArea()
        {
            string userName = User.Identity.Name;
            var registeredUser = _db.MyRegisteredUsers.FirstOrDefault(ru => ru.Email == userName);

            return View(registeredUser);
        }

    }
}
