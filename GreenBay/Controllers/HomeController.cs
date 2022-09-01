using GreenBay.Context;
using GreenBay.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GreenBay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _db;

        public HomeController(ApplicationContext db)
        {
            _db = db;
        }

        [Route("/hello")]
        public IActionResult Hello()
        {
            User user = _db.Users.FirstOrDefault();
            return View(user);
        }
    }
}
