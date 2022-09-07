using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenBay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly ISecurityService _securityService;
        private readonly ISellService _sellService;
        private readonly IStoreService _storeService;
        private readonly IBuyService _buyService;

        public HomeController(ApplicationContext db, ISecurityService securityService
            , ISellService sellService, IStoreService storeService, IBuyService buyService)
        {
            _db = db;
            _securityService = securityService;
            _sellService = sellService;
            _storeService = storeService;
            _buyService = buyService;
        }

        [HttpGet("/hello")]
        public IActionResult Hello()
        {
            Random r = new();
            List<User> users = _db.Users.ToList();
            User user = users.ElementAt(r.Next(users.Count));
            return View(user);
        }

        [HttpGet("/buyable")]
        public IActionResult ListBuyableItems(string search)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            int page = _securityService.ReadPageFromCookies(HttpContext.Request.Cookies);
            int itemCount = _securityService.ReadItemCountFromCookies(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            List<ItemInfoDto> items;
            if (search == null || search == String.Empty)
            {
                items = _sellService.ListAllSellableItems(user.Id, page, itemCount);
            }
            else
            {
                items = _sellService.ListAllSellableItems(user.Id, 1, int.MaxValue);
                items = _storeService.SearchText(items, search);
            }
            ViewBag.money = user.Dollars;
            ViewBag.name = user.Name;
            ViewBag.page = page;
            ViewBag.itemCount = itemCount;
            return View(items);
        }

        [HttpGet("/all")]
        public IActionResult ListAllItems(string search)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            int page = _securityService.ReadPageFromCookies(HttpContext.Request.Cookies);
            int itemCount = _securityService.ReadItemCountFromCookies(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            List<ItemInfoDto> items;
            if (search == null || search == String.Empty)
            {
                items = _sellService.ListAllSellableItems(user.Id, page, itemCount);
            }
            else
            {
                items = _sellService.ListAllSellableItems(user.Id, 1, int.MaxValue);
                items = _storeService.SearchText(items, search);
            }
            ViewBag.money = user.Dollars;
            ViewBag.name = user.Name;
            ViewBag.page = page;
            ViewBag.itemCount = itemCount;
            return View(items);
        }

        [HttpGet("/sellable")]
        public IActionResult ListSellableItems(string search)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            int page = _securityService.ReadPageFromCookies(HttpContext.Request.Cookies);
            int itemCount = _securityService.ReadItemCountFromCookies(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            List<ItemInfoDto> items;
            if (search == null || search == String.Empty)
            {
                items = _sellService.ListAllSellableItems(user.Id, page, itemCount);
            }
            else
            {
                items = _sellService.ListAllSellableItems(user.Id, 1, int.MaxValue);
                items = _storeService.SearchText(items, search);
            }   
            ViewBag.money = user.Dollars;
            ViewBag.name = user.Name;
            ViewBag.page = page;
            ViewBag.itemCount = itemCount;
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
            UserLogin userLogin = new(name, password);
            ResponseLoginObjectDto response = _securityService.Authenticate(userLogin);
            if (response.StatusCode == 200)
            {
                IResponseCookies cookies = HttpContext.Response.Cookies;
                string Token = response.ResponseLoginObjectOutput.Token;
                cookies.Append("Authorization", Token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            }
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
            UserCreate userCreate = new(userName, email, password, role, dollars);
            ResponseObject response = _securityService.CheckDuplicity(userCreate);
            if (response.StatusCode == 201)
            {
                User user = _storeService.CreateUser(userCreate);
                string Token = _securityService.GenerateToken(user);
                ViewBag.token = Token;
                IResponseCookies cookies = HttpContext.Response.Cookies;
                cookies.Append("Authorization", Token, new CookieOptions() {HttpOnly = true, SameSite = SameSiteMode.Strict });
            }
            return View(response);
        }

        [HttpGet("/add")]
        public IActionResult AddProduct()
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return View();
        }

        [HttpPost("/add")]
        public IActionResult AddProductResult(string name, string description, string imageUrl, int price)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ItemCreate item = new(name, description, imageUrl, price);
            ResponseItemObjectDto response = _storeService.CreateItem(item, user);
            return View(response);
        }

        [HttpGet("/email")]
        public IActionResult SendEmailWithCreds()
        {
            return View();
        }

        [HttpPost("/email")]
        public IActionResult SendEmailWithCredsResult(string email)
        {
            Credentials credentials = new(email);
            ResponseObject result = _securityService.ValidateCredentials(null, credentials);
            return View("ResponseObjectPage", result);
        }

        [HttpGet("/info")]
        public IActionResult UserInfo()
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            UserInfoFullDto result = _sellService.UserInfoDetailed(user.Id, null);
            return View(result);
        }

        [HttpGet("/money")]
        public IActionResult ManageMoney()
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ViewBag.name = user.Name;
            ViewBag.money = user.Dollars;
            return View(user);
        }

        [HttpPost("/money")]
        public IActionResult ManageMoneyResult(string action, int amount)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ResponseObject result = _storeService.ManageMoney(new DollarsManage(amount, action), user.Id);   
            return View("ResponseObjectPage", result);
        }

        [HttpGet("/users")]
        public IActionResult AllUsersInfo(int page, int itemCount)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (user.Role != "admin")
            {
                return Unauthorized("Unauthorized, you must have admin permissions to show this page.");
            }
            List<UserInfoDto> users = _securityService.ListAllUsers(page,itemCount);
            return View(users);
        }

        [HttpGet("/bid")]
        public IActionResult Bid()
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return View(user);
        }

        [HttpPost("/bid")]
        public IActionResult BidResult(int itemId, int bid)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ResponseItemObjectDto result = _buyService.Bid(new ItemBid(itemId, bid), user);
            return View(result);
        }

        [HttpPost("/sell-withdraw")]
        public IActionResult SellOrWithdraw(int itemId, string action)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ResponseObject result = _buyService.SellOrWithdraw(new ItemAction(itemId, action), user);
            return View("ResponseObjectPage", result);
        }

        [HttpPost("/buy-withdraw")]
        public IActionResult BuyOrWithdraw(int itemId, string action)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ResponseObject result = _buyService.BuyOrWithdraw(new ItemAction(itemId, action), user);
            return View("ResponseObjectPage", result);
        }

        [HttpPost("/set_page")]
        public IActionResult SetPage(int page, string redirectTo)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            IResponseCookies cookies = HttpContext.Response.Cookies;
            cookies.Append("Page", page.ToString(), new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            return RedirectToAction(redirectTo);
        }

        [HttpPost("/set_item_count")]
        public IActionResult SetItemCount(int itemCount, string redirectTo)
        {
            int userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return Unauthorized("Unauthorized, please log in.");
            }
            IResponseCookies cookies = HttpContext.Response.Cookies;
            cookies.Append("ItemCount", itemCount.ToString(), new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            return RedirectToAction(redirectTo);
        }
    }
}
