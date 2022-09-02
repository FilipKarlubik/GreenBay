using GreenBay.Models;
using GreenBay.Models.DTOs;
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

        [HttpGet("list")] // all items, include already bought
        [Authorize(Roles ="admin")]
        public ActionResult ListAllProducts(int page, int itemCount)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity); 
                return Ok(_sellService.ListAllItems(page, itemCount));
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpGet("list/buyable")] //not yet bought, no your own, with price lower than your money amount
        public ActionResult ListAllBuyableProducts(int page, int itemCount)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok(_sellService.ListAllBuyableItems(user.Id, page, itemCount));
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpGet("list/sellable")] //all not yet bought include your own
        public ActionResult ListAllSellableProducts(int page, int itemCount)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                return Ok(_sellService.ListAllSellableItems(user.Id, page, itemCount));
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpGet("item")] // info about one item with it specified in pathvar 
        public ActionResult ItemInfo(int id)
        {
            ResponseItemObjectDto response = _sellService.ItemInfo(id);       
            if (response.StatusCode == 200)
            {
                return StatusCode(response.StatusCode, response.ItemInfo);
            }
            return StatusCode(response.StatusCode, new { error = response.Message });

        }

        [HttpPost("create")]
        public ActionResult CreateNewItem([FromBody] ItemCreate newItem)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseItemObjectDto response = _storeService.CreateItem(newItem, user);
                if (response.StatusCode == 201)
                {
                    return StatusCode(response.StatusCode, new
                    {
                        status = response.Message,
                        item = response.ItemInfo
                    });
                }
                return StatusCode(response.StatusCode, new { error = response.Message });
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpPost("bid")]
        public ActionResult BidOnItem([FromBody] ItemBid item)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseItemObjectDto response = _buyService.Bid(item, user);
                if (response.StatusCode == 200)
                {
                    return StatusCode(response.StatusCode, new { status = response.Message,
                    item_info = response.ItemInfo
                    });
                }
                return StatusCode(response.StatusCode, new { error = response.Message,
                item_info = response.ItemInfo});
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpPost("sell-withdraw")] //withdraw after 7 days,  sell anytime
        public ActionResult SellOrWithdrawItemIfNotReachPrice([FromBody] ItemAction itemAction)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseObject response = _buyService.SellOrWithdraw(itemAction, user);
                if (response.StatusCode == 200)
                {
                    return StatusCode(response.StatusCode, new { status = response.Message });
                }
                return StatusCode(response.StatusCode, new { error = response.Message });
            }
            return Unauthorized(new { error = "Not valid token" });
        }

        [HttpPost("buy-withdraw")] // after 10 days - buy for at least half price
        public ActionResult BuyForAtLeastHalfOrWithdraw([FromBody] ItemAction itemAction)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                User user = _securityService.DecodeUser(identity);
                ResponseObject response = _buyService.BuyOrWithdraw(itemAction, user);
                if (response.StatusCode == 200)
                {
                    return StatusCode(response.StatusCode, new { status = response.Message });
                }
                return StatusCode(response.StatusCode, new { error = response.Message });
            }
            return Unauthorized(new { error = "Not valid token" });
        }
    }
}
