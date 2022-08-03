using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Collections.Generic;

namespace GreenBay.Services
{
    public interface ISellService
    {
        List<ItemInfoDto> ListAllItems(int page, int itemCount);
        List<ItemInfoDto> ListAllBuyableItems(int id, int page, int itemCount);
        List<ItemInfoDto> ListAllSellableItems(int id, int page, int itemCount);
        UserInfoFullDto UserInfoDetailed(int id);
        ResponseItemObjectDto ItemInfo(int id);
        ItemInfoDto GenerateItemInfo(Item item);
    }
}
