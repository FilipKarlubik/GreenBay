using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
   
        [Authorize(Roles = "admin")]
        [HttpGet("show")] //list all existing users 
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
        public async Task<ActionResult> UserInfoAsync()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            string token = await HttpContext.GetTokenAsync("access_token");
            User user = _securityService.DecodeUser(identity);
            if (user != null)
            {
                return Ok(_sellService.UserInfoDetailed(user.Id, token));
            }
            return NotFound(new { error = "User not found." });
        }

        [AllowAnonymous]
        [HttpPost("email")] // send an email with credentials    
        public ActionResult SendEmail([FromBody] Credentials credentials)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User user = null;
            if (identity.IsAuthenticated)
            {
                user = _securityService.DecodeUser(identity);
            }
            ResponseObject response = _securityService.ValidateCredentials(user, credentials);
            return StatusCode(response.StatusCode, new { status = response.Message });
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("encrypt_passwords")]
        public ActionResult EncryptPasswords()
        {
            ResponseObject response = _securityService.EncryptPasswords();
            if( response.StatusCode != 200 )
            {
                return StatusCode(response.StatusCode, new { error = response.Message });
            }
            return StatusCode(response.StatusCode, new { status = response.Message });
        }
    }
}
