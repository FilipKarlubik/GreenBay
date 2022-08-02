using GreenBay.Models.DTOs;
using System.Collections.Generic;

namespace GreenBay.Services
{
    public interface ISellService
    {
        List<ItemInfoDto> ListAllItems();
        List<ItemInfoDto> ListAllBuyableItems(int id);
        List<ItemInfoDto> ListAllSellableItems(int id);
    }
}
