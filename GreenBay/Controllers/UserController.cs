using GreenBay.Models;
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

        public UserController(ISecurityService securityService, IStoreService storeService)
        {
            _securityService = securityService;
            _storeService = storeService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLogin userLogin)
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
        public ActionResult Create([FromBody] UserCreate userCreate)
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
        [AllowAnonymous]
        [Authorize(Roles ="admin")]
        public ActionResult ShowAllUsers()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok(_securityService.ListAllUsers());
            }
            return Unauthorized("Not valid token");
        }

        [HttpPost("money")]
        public ActionResult MoneyChange([FromBody] DollarsManage dollars)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User user = _securityService.DecodeUser(identity);
            if (user != null)
            {
                ResponseObject response = _storeService.ManageMoney(dollars, user.Id);
                return StatusCode(response.StatusCode,response.Message);
            }
            return Unauthorized("Not valid token");
        }
    }
}
