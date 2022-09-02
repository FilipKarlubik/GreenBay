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
        private readonly IStoreService _storeService;

        public HomeController(ApplicationContext db, ISecurityService securityService
            , ISellService sellService, IStoreService storeService)
        {
            _db = db; 
            _securityService = securityService;
            _sellService = sellService;
            _storeService = storeService;
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

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/login")]
        public IActionResult LoginResult(string name, string password)
        {
            UserLogin userLogin = new UserLogin(name, password);
            ResponseLoginObjectDto response = _securityService.Authenticate(userLogin);  
            return View(response);
        }

        [HttpGet("/create")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost("/create")]
        public IActionResult CreateUserResult(string userName, string email, string password, string role, int dollars)
        {
            UserCreate userCreate = new UserCreate(userName, email, password, role, dollars);
            ResponseObject response = _securityService.CheckDuplicity(userCreate);
            if (response.StatusCode == 201)
            {
                User user = _storeService.CreateUser(userCreate);
                ViewBag.token = _securityService.GenerateToken(user);
            }
            return View(response);
        }
    }
}
