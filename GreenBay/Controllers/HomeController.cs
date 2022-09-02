using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GreenBay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly ISecurityService _securityService;
        private readonly ISellService _sellService;

        public HomeController(ApplicationContext db, ISecurityService securityService, ISellService sellService)
        {
            _db = db; 
            _securityService = securityService;
            _sellService = sellService;
        }

        [Route("/hello")]
        public IActionResult Hello()
        {
            Random r = new Random();
            List<User> users = _db.Users.ToList();
            User user = users.ElementAt(r.Next(users.Count));
            return View(user);
        }

        [Authorize]
        [Route("/buy")]
        public IActionResult ListBuyableItems(int page, int itemCount)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User user = _securityService.GetUserFromDB(_securityService.DecodeUser(identity).Id);
            List<ItemInfoDto> items = _sellService.ListAllBuyableItems(user.Id, page, itemCount);
            ViewBag.money = user.Dollars;
            ViewBag.name = user.Name;
            return View(items);
        }
    }
}
