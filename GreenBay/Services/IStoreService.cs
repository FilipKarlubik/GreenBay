using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Collections.Generic;

namespace GreenBay.Services
{
    public interface IStoreService
    {
        User CreateUser(UserCreate user);
        ResponseItemObjectDto CreateItem(ItemCreate itemNew, User user);
        ResponseObject ManageMoney(DollarsManage dollars, int id);
        List<ItemInfoDto> SearchText(List<ItemInfoDto> items, string search);
    }
}
