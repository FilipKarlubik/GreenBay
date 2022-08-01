using GreenBay.Models;
using GreenBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;

namespace GreenBay.Controllers
{
    [Route("user")]
    [Authorize(Roles ="admin")]
    public class UserController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IStoreService _storeService;

        public UserController(ISecurityService securityService, IStoreService storeService)
        {
            _securityService = securityService;
            _storeService = storeService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = _securityService.Authenticate(userLogin);
            
            if (user != null)
            {
                var token = _securityService.GenerateToken(user);
                return Ok(token);
            }
            return NotFound("User not found");
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult Create([FromBody] UserCreate userCreate)
        {
            User user = _securityService.CheckDuplicity(userCreate);
            if (user != null)
            {
                var token = _securityService.GenerateToken(user);
                _storeService.CreateUser(user);
                return Ok(token);
            }
            return BadRequest("Not valid parameters or User with that name already exists");
        }
        
        [HttpGet("show")]
        public IActionResult ShowAllUsers()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok("here they are " + user.Name);
            }
            return Unauthorized("Not valid token");
        }
    }
}
