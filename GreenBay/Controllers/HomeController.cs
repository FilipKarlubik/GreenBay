using GreenBay.Context;
using GreenBay.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            Random r = new Random();
            List<User> users = _db.Users.ToList();
            User user = users.ElementAt(r.Next(users.Count));
            return View(user);
        }
    }
}
