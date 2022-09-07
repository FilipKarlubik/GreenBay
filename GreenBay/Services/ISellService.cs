using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Collections.Generic;

namespace GreenBay.Services
{
    public interface ISellService
    {
        List<ItemInfoDto> ListAllItems(int page, int itemCount, string sortBy);
        List<ItemInfoDto> ListAllBuyableItems(int id, int page, int itemCount, string sortBy);
        List<ItemInfoDto> ListAllSellableItems(int id, int page, int itemCount, string sortBy);
        UserInfoFullDto UserInfoDetailed(int id, string token, string sortBy);
        ResponseItemObjectDto ItemInfo(int id);
        ItemInfoDto GenerateItemInfo(Item item);
    }
}
