using GreenBay.Models;
using GreenBay.Models.DTOs;

namespace GreenBay.Services
{
    public interface IBuyService
    {
        ResponseItemObjectDto Bid(ItemBid item, User user);
        ResponseObject SellOrWithdraw(ItemAction itemAction, User user);
        ResponseObject BuyOrWithdraw(ItemAction itemAction, User user);
    }
}
