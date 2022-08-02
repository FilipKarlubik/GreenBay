using GreenBay.Models;

namespace GreenBay.Services
{
    public interface IBuyService
    {
        ResponseObject Bid(ItemBid item, User user);
        ResponseObject SellOrWithdraw(ItemAction itemAction, User user);
        ResponseObject BuyOrWithdraw(ItemAction itemAction, User user);
    }
}
