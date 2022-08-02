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


        public User CreateUser(UserCreate userCreate)
        {
            string role = "user";
            if (userCreate.Role.ToLower() == "admin") role = "admin";
            User user = new User()
            {
                Name = userCreate.UserName,Password = userCreate.Password,Email = userCreate.Email        
            , Dollars = userCreate.Dollars, Role = role};

            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public ResponseObject CreateItem(ItemCreate itemNew, User user)
        {
            _db.Items.Add(new Item()
            {
                Name = itemNew.Name,
                Description = itemNew.Description,
                Price = itemNew.Price,
                ImageUrl = itemNew.ImageUrl,
                UserId = user.Id
            });
            _db.SaveChanges();   
            return new ResponseObject(200, $"New item {itemNew.Name} has been created, selling by {user.Name}");
        }

        public ResponseObject ManageMoney(DollarsManage dollars, int id)
        {
            if (dollars.Amount <0)
            {
                return new ResponseObject(400, "Can not perform operation with negative amount of money");
            }
            User user = _db.Users.Find(id);
            if (dollars.Action.ToLower() == "increase")
            {
                user.Dollars += dollars.Amount;
                _db.SaveChanges();
                return new ResponseObject(200, $"{user.Name}, your new amount of Dollars is {user.Dollars}");
            }
            if(dollars.Action.ToLower() == "decrease")
            {
                if(user.Dollars - dollars.Amount < 0)
                {
                    return new ResponseObject(409, $"{user.Name}, not enough money to take back, you have only {user.Dollars} Dollars");
                }
                else
                {
                    user.Dollars -= dollars.Amount;
                    _db.SaveChanges();
                    return new ResponseObject(200, $"{user.Name}, your new amount of Dollars is {user.Dollars}");
                }
            }
            else
            {
                return new ResponseObject(400, "Not alowed action " + dollars.Action);
            }
        }
    }
}
