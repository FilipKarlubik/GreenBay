using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace GreenBay.Controllers
{
    [ApiController]
    [Route("user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IStoreService _storeService;
        private readonly ISellService _sellService;

        public UserController(ISecurityService securityService, IStoreService storeService, ISellService sellService)
        {
            _securityService = securityService;
            _storeService = storeService;
            _sellService = sellService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLogin userLogin)
        {
            ResponseLoginObjectDto response = _securityService.Authenticate(userLogin);
            if (response.StatusCode == 200)
            {
                return StatusCode(response.StatusCode, response.ResponseLoginObjectOutput);
            }
            return StatusCode(response.StatusCode, new { error = response.Message });
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public ActionResult Create([FromBody] UserCreate userCreate)
        {
            ResponseObject response = _securityService.CheckDuplicity(userCreate);
            if (response.StatusCode == 201)
            {
                User user = _storeService.CreateUser(userCreate);
                var token = _securityService.GenerateToken(user);
                return StatusCode(response.StatusCode,new { status = response.Message, token = token });
            }
            return StatusCode(response.StatusCode, new { error = response.Message });
        }
        
        [HttpGet("show")] //list all existing users 
        [AllowAnonymous]
        [Authorize(Roles ="admin")]
        public ActionResult ShowAllUsers(int page, int itemCount)
        {           
            return Ok(_securityService.ListAllUsers(page, itemCount));
        }

        [HttpPost("money")] // change your money amount - add or remove money
        public ActionResult MoneyChange([FromBody] DollarsManage dollars)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User user = _securityService.DecodeUser(identity);
            if (user != null)
            {
                ResponseObject response = _storeService.ManageMoney(dollars, user.Id);
                return StatusCode(response.StatusCode,new { status = response.Message });
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpGet("info")] // logged user info
        public ActionResult UserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User user = _securityService.DecodeUser(identity);
            if (user != null)
            {
                return Ok(_sellService.UserInfoDetailed(user.Id));
            }
            return NotFound(new { error = "User not found." });
        }
    }
}
