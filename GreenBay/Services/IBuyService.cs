using GreenBay.Models;

namespace GreenBay.Services
{
    public interface IBuyService
    {
        ResponseObject Bid(ItemBid item, User user);
    }
}
