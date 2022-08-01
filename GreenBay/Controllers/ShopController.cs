using GreenBay.Models;
using GreenBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GreenBay.Controllers
{
    [Route("shop")]
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ISecurityService _securityService;

        public ShopController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpGet("list")]
        public IActionResult ListAllProducts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok("here they are " + user.Name);
            }
            return Ok("here they are :) ");
        }
    }
}
