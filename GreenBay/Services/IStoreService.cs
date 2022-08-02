using GreenBay.Models;

namespace GreenBay.Services
{
    public interface IStoreService
    {
        User CreateUser(UserCreate user);
        ResponseObject CreateItem(ItemCreate itemNew, User user);
        ResponseObject ManageMoney(DollarsManage dollars, int id);
    }
}
