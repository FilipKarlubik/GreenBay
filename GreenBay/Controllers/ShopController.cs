using GreenBay.Models;
using GreenBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GreenBay.Controllers
{
    [ApiController]
    [Route("shop")]
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IStoreService _storeService;
        private readonly ISellService _sellService;
        private readonly IBuyService _buyService;

        public ShopController(ISecurityService securityService, IStoreService storeService, ISellService sellService, IBuyService buyService)
        {
            _securityService = securityService;
            _storeService = storeService;
            _sellService = sellService;
            _buyService = buyService;
        }

        [HttpGet("list")]
        [Authorize(Roles ="admin")]
        public ActionResult ListAllProducts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity); 
                return Ok(_sellService.ListAllItems());
            }
            return Unauthorized("Not valid token");
        }

        [HttpGet("list/buyable")]
        public ActionResult ListAllBuyableProducts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok(_sellService.ListAllBuyableItems(user.Id));
            }
            return Unauthorized("Not valid token");
        }

        [HttpGet("list/sellable")]
        public ActionResult ListAllSellableProducts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok(_sellService.ListAllSellableItems(user.Id));
            }
            return Unauthorized("Not valid token");
        }

        [HttpPost("create")]
        public ActionResult CreateNewItem([FromBody] ItemCreate newItem)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseObject response = _storeService.CreateItem(newItem, user.Id);
                return StatusCode(response.StatusCode, response.Message);
            }
            return Unauthorized("Not valid token");
        }

        [HttpPost("bid")]
        public ActionResult BidOnItem([FromBody] ItemBid item)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseObject response = _buyService.Bid(item, user);
                return StatusCode(response.StatusCode, response.Message);
            }
            return Unauthorized("Not valid token");
        }
    }
}
