﻿using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [Route("/buyable")]
        public IActionResult ListBuyableItems(int page, int itemCount)
        {
            var userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return BadRequest("Unauthorized");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            List<ItemInfoDto> items = _sellService.ListAllBuyableItems(user.Id, page, itemCount);
            ViewBag.money = user.Dollars;
            ViewBag.name = user.Name;
            return View(items);
        }

        [Route("/all")]
        public IActionResult ListAllItems(int page, int itemCount)
        {
            var userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return BadRequest("Unauthorized");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            List<ItemInfoDto> items = _sellService.ListAllItems(page, itemCount);
            ViewBag.money = user.Dollars;
            ViewBag.name = user.Name;
            return View(items);
        }

        [Route("/sellable")]
        public IActionResult ListSellableItems(int page, int itemCount)
        {
            var userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return BadRequest("Unauthorized");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            List<ItemInfoDto> items = _sellService.ListAllSellableItems(user.Id, page, itemCount);
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
            if (response.StatusCode == 200)
            {
                var cookies = HttpContext.Response.Cookies;
                var Token = response.ResponseLoginObjectOutput.Token;
                cookies.Append("Authorization", Token);
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
            UserCreate userCreate = new UserCreate(userName, email, password, role, dollars);
            ResponseObject response = _securityService.CheckDuplicity(userCreate);
            if (response.StatusCode == 201)
            {
                User user = _storeService.CreateUser(userCreate);
                var Token = _securityService.GenerateToken(user);
                ViewBag.token = Token;
                var cookies = HttpContext.Response.Cookies;
                cookies.Append("Authorization", Token);
            }
            return View(response);
        }


        [HttpGet("/add")]
        public IActionResult AddProduct()
        {
            var userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return BadRequest("Unauthorized");
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
            var userID = _securityService.CheckJWTCookieValidityReturnsUserID(HttpContext.Request.Cookies);
            if (userID == -1)
            {
                return BadRequest("Unauthorized");
            }
            User user = _securityService.GetUserFromDB(userID);
            if (user == null)
            {
                return NotFound("User not found");
            }
            ItemCreate item = new ItemCreate(name, description, imageUrl, price);
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
            Credentials credentials = new Credentials(email);
            ResponseObject result = _securityService.ValidateCredentials(null, credentials);
            return View(result);
        }
    }
}
