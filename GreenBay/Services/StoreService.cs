using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using System;
using System.Linq;

namespace GreenBay.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationContext _db;
        private readonly ISellService _sellService;

        public StoreService(ApplicationContext db, ISellService sellService)
        {
            _db = db;
            _sellService = sellService;
        }


        public User CreateUser(UserCreate userCreate)
        {
            string role = "user";
            if (userCreate.Role.ToLower() == "admin") role = "admin";
            User user = new User()
            {
                Name = userCreate.UserName,Password = Constants.EncryptPassword(userCreate.Password),Email = userCreate.Email        
            , Dollars = userCreate.Dollars, Role = role};

            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public ResponseItemObjectDto CreateItem(ItemCreate itemNew, User user)
        {
            if (!_db.Users.Any(u => u.Id.Equals(user.Id)))
            {
                return new ResponseItemObjectDto(404, $"User with Name: {user.Name} is not in database.");
            }
            if (itemNew.Name == null || itemNew.Name == string.Empty)
            {
                return new ResponseItemObjectDto(400, "Not valid name.");
            }
            if (itemNew.Price < 1)
            {
                return new ResponseItemObjectDto(400, $"Not valid price :{itemNew.Price}.");
            }
            if (itemNew.Description == null || itemNew.Description == string.Empty)
            {
                return new ResponseItemObjectDto(400, "Not valid description.");
            }
            Uri uriResult;
            bool validUrl = Uri.TryCreate(itemNew.ImageUrl, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (validUrl == false)
            {
                return new ResponseItemObjectDto(400, "Not valid image Url.");
            }
            if (_db.Items.Any(i => i.Name.Equals(itemNew.Name)))
            {
                return new ResponseItemObjectDto(409, $"Item with name {itemNew.Name} already exists.");
            }
            _db.Items.Add(new Item()
            {
                Name = itemNew.Name,
                Description = itemNew.Description,
                Price = itemNew.Price,
                ImageUrl = itemNew.ImageUrl,
                UserId = user.Id
            });
            _db.SaveChanges();
            Item item = _db.Items.FirstOrDefault(x => x.Name == itemNew.Name);

            return new ResponseItemObjectDto(201, "New item has been created", _sellService.GenerateItemInfo(item));
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
