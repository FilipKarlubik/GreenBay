using GreenBay.Models;

namespace GreenBay.Services
{
    public interface IStoreService
    {
        void CreateUser(User user);
        void CreateItem(ItemCreate itemNew, int userId);
        ResponseObject ManageMoney(DollarsManage dollars, int id);
    }
}
