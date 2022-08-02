using GreenBay.Context;
using GreenBay.Models;

namespace GreenBay.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationContext _db;

        public StoreService(ApplicationContext db)
        {
            _db = db;
        }


        public void CreateUser(User user)
        {
           _db.Users.Add(user);
           _db.SaveChanges();
        }

        public void CreateItem(ItemCreate itemNew, int userId)
        {
            _db.Items.Add(new Item()
            {
                Name = itemNew.Name,
                Description = itemNew.Description,
                Price = itemNew.Price,
                ImageUrl = itemNew.ImageUrl,
                UserId = userId
            });
            _db.SaveChanges();
        }

        public ResponseObject ManageMoney(DollarsManage dollars, int id)
        {
            if (dollars.Amount <0)
            {
                return new ResponseObject(400, "Can not perform operation with negative amount of money");
            }
            string message = "Your new amount of Dollars is ";
            User user = _db.Users.Find(id);
            if(dollars.Action.ToLower() == "increase")
            {
                user.Dollars += dollars.Amount;
                _db.SaveChanges();
                return new ResponseObject(200, message + user.Dollars);
            }
            if(dollars.Action.ToLower() == "decrease")
            {
                if(user.Dollars - dollars.Amount < 0)
                {
                    return new ResponseObject(409, "Not enough money to take back, you have only " + user.Dollars);
                }
                else
                {
                    user.Dollars -= dollars.Amount;
                    _db.SaveChanges();
                    return new ResponseObject(200, message + user.Dollars);
                }
            }
            else
            {
                return new ResponseObject(400, "Not alowed action " + dollars.Action);
            }
        }
    }
}
