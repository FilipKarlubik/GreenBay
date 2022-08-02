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
                ResponseObject response = _storeService.CreateItem(newItem, user);
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

        [HttpPost("sell-withdraw")] //withdraw after 7 days,  sell anytime
        public ActionResult SellOrWithdrawItemIfNotReachPrice([FromBody] ItemAction itemAction)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseObject response = _buyService.SellOrWithdraw(itemAction, user);
                return StatusCode(response.StatusCode, response.Message);
            }
            return Unauthorized("Not valid token");
        }

        [HttpPost("buy-withdraw")] // after 10 days - buy for at least half price
        public ActionResult BuyForAtLeastHalfOrWithdraw([FromBody] ItemAction itemAction)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseObject response = _buyService.BuyOrWithdraw(itemAction, user);
                return StatusCode(response.StatusCode, response.Message);
            }
            return Unauthorized("Not valid token");
        }
    }
}
