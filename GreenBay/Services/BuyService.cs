using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Linq;

namespace GreenBay.Services
{
    public class BuyService : IBuyService
    {
        private readonly ApplicationContext _db;
        private readonly ISellService _sellService;

        public BuyService(ApplicationContext db, ISellService sellService)
        {
            _db = db;
            _sellService = sellService;
        }

        public ResponseItemObjectDto Bid(ItemBid item, User user)
        {
            if (item == null)
            {
                return new ResponseItemObjectDto(400, "Not valid input object");
            }
            if (item.ItemId < 1)
            {
                return new ResponseItemObjectDto(400, $"Id:{item.ItemId} of item is not valid");
            }
            Item itemToBid = _db.Items.FirstOrDefault(i => i.Id.Equals(item.ItemId));
            if (itemToBid == null)
            {
                return new ResponseItemObjectDto(404, $"Item with id:{item.ItemId} is not in database");
            } 
            if (itemToBid.UserId.Equals(user.Id))
            {
                return new ResponseItemObjectDto(409, $"{user.Name}, you can not buy your own product!"
                    , _sellService.GenerateItemInfo(itemToBid));
            }
            if (itemToBid.BoughtById > 0)
            {
                string boughtBy = _db.Users.FirstOrDefault(u => u.Id.Equals(itemToBid.BoughtById)).Name;
                return new ResponseItemObjectDto(404, $"Item with id:{item.ItemId} is already bought by {boughtBy}."
                    , _sellService.GenerateItemInfo(itemToBid));
            }
            if (itemToBid.Bid >= item.Bid)
            {
                return new ResponseItemObjectDto(400, $"Actual bid: {itemToBid.Bid} of product {itemToBid.Name} " +
                    $"is higher or equal as yours: {item.Bid}, you must higher your bid. Full price is {itemToBid.Price}."
                    , _sellService.GenerateItemInfo(itemToBid));
            }
            user = _db.Users.FirstOrDefault(u => u.Id.Equals(user.Id));
            if (user == null)
            {
                return new ResponseItemObjectDto(404, "User is not in database");
            }
            if (itemToBid.BidById.Equals(user.Id))
            { 
                if(user.Dollars < item.Bid - itemToBid.Bid)
                {
                    return new ResponseItemObjectDto(404, $"{user.Name} you have not enough money: {user.Dollars} to rise the bid: {item.Bid - itemToBid.Bid} is needed."
                        , _sellService.GenerateItemInfo(itemToBid));
                }
                else
                {
                    user.Dollars -= (item.Bid - itemToBid.Bid);
                    itemToBid.Bid = item.Bid;
                    if (itemToBid.Bid >= itemToBid.Price)
                    {
                        itemToBid.BoughtById = user.Id;
                        User giveMoneyForSellToUser = _db.Users.FirstOrDefault(u => u.Id.Equals(itemToBid.UserId));
                        giveMoneyForSellToUser.Dollars += itemToBid.Bid;
                        _db.SaveChanges();
                        return new ResponseItemObjectDto(200, $"{user.Name}, you have bought item {itemToBid.Name} for {itemToBid.Bid} dollars."
                            ,_sellService.GenerateItemInfo(itemToBid));
                    }
                    else
                    {
                        _db.SaveChanges();
                        return new ResponseItemObjectDto(200, $"{user.Name}, you have risen the bit of item {itemToBid.Name} to {itemToBid.Bid} dollars."
                            , _sellService.GenerateItemInfo(itemToBid));
                    }
                }
            }
            else
            {
                if (user.Dollars < item.Bid)
                {
                    return new ResponseItemObjectDto(404, $"{user.Name}, you have not enough money: {user.Dollars} to rise the bid: {item.Bid} is needed."
                        , _sellService.GenerateItemInfo(itemToBid));
                }
                if (itemToBid.Bid > 0)
                {
                    User giveMoneyBackToUser = _db.Users.FirstOrDefault(u => u.Id.Equals(itemToBid.BidById));
                    giveMoneyBackToUser.Dollars += itemToBid.Bid;     
                }
                itemToBid.Bid = item.Bid;
                user.Dollars -= item.Bid;
                itemToBid.BidById = user.Id;
                if(itemToBid.Bid >= itemToBid.Price)
                {
                    itemToBid.BoughtById = user.Id;
                    User giveMoneyForSellToUser = _db.Users.FirstOrDefault(u => u.Id.Equals(itemToBid.UserId));
                    giveMoneyForSellToUser.Dollars += itemToBid.Bid;
                    _db.SaveChanges();
                    return new ResponseItemObjectDto(200, $"{user.Name}, you have bought item {itemToBid.Name} for {itemToBid.Bid} dollars."
                        , _sellService.GenerateItemInfo(itemToBid));
                }
                else
                {
                    _db.SaveChanges();
                    return new ResponseItemObjectDto(200, $"{user.Name}, you have risen the bit of item {itemToBid.Name} to {itemToBid.Bid} dollars."
                        , _sellService.GenerateItemInfo(itemToBid));
                }
            }
        }

        public ResponseObject SellOrWithdraw(ItemAction itemAction, User userFromToken)
        {
            if (itemAction.Action != "sell" && itemAction.Action != "withdraw")
            {
                return new ResponseObject(400, $"Action must be sell/withdraw, no {itemAction.Action}!");
            }
            User user = _db.Users.FirstOrDefault(u => u.Id.Equals(userFromToken.Id));
            if (user == null)
            {
                return new ResponseObject(404, "User not in database");
            }
            Item item = _db.Items.FirstOrDefault(i => i.Id == itemAction.Id);
            if (item == null)
            {
                return new ResponseObject(404, $"Item with ID: {itemAction.Id} not in database.");
            }
            if(item.BoughtById > 0)
            {
                return new ResponseObject(409, $"Product has been already sold to {_db.Users.FirstOrDefault(u => u.Id.Equals(item.BoughtById)).Name}.");
            }
            if (item.UserId != user.Id)
            {
                return new ResponseObject(409, $"{user.Name}, you can not sell or withdraw product witch doesnt belong to you.");
            }
            if(itemAction.Action == "withdraw")
            {
                if(item.CreatedAt.AddDays(7) > System.DateTime.Now)
                {
                    return new ResponseObject(406, $"{user.Name}, you can not withdraw item before one week from advertising : {item.CreatedAt.AddDays(7)}");
                }
                else
                {
                    if (item.Bid > 0)
                    {
                        User userToGiveMoneyBack = _db.Users.FirstOrDefault(u => u.Id == item.BidById);
                        userToGiveMoneyBack.Dollars += item.Bid;
                    }
                    _db.Items.Remove(item);
                    _db.SaveChanges();
                    return new ResponseObject(200, $"{user.Name} ,you have removed item {item.Name} from GreenBay");
                }
            }
            else // Action = sell
            {
                if (item.Bid > 0)
                {
                    user.Dollars += item.Bid;
                    User userBoughtThisItem = _db.Users.FirstOrDefault(u => u.Id == item.BidById);
                    item.BoughtById = userBoughtThisItem.Id;
                    _db.SaveChanges();
                    return new ResponseObject(200, $"{user.Name}, you have sold product {item.Name} to {userBoughtThisItem.Name} for {item.Bid} dollars.");

                }
                else
                {
                    return new ResponseObject(409, $"{user.Name}, you can not sell this product, because nobody has offer for it");
                }
            }
        }

        public ResponseObject BuyOrWithdraw(ItemAction itemAction, User userFromToken)
        {
            if (itemAction.Action != "buy" && itemAction.Action != "withdraw")
            {
                return new ResponseObject(400, $"Action must be buy/withdraw, no {itemAction.Action}!");
            }
            User user = _db.Users.FirstOrDefault(u => u.Id.Equals(userFromToken.Id));
            if (user == null)
            {
                return new ResponseObject(404, "User not in database");
            }
            Item item = _db.Items.FirstOrDefault(i => i.Id == itemAction.Id);
            if (item == null)
            {
                return new ResponseObject(404, $"Item with ID: {itemAction.Id} not in database.");
            }
            if (item.BoughtById > 0)
            {
                return new ResponseObject(410, $"Product has been already sold to {_db.Users.FirstOrDefault(u => u.Id.Equals(item.BoughtById)).Name}.");
            }
            if (item.BidById != user.Id)
            {
                return new ResponseObject(409, $"{user.Name}, you dont have bid on this product.");
            }
            if (item.CreatedAt.AddDays(10) > System.DateTime.Now)
            {
                return new ResponseObject(409, $"{user.Name}, you can not perform buy/withdraw action on this item before 10 days from advertising : {item.CreatedAt.AddDays(10)}");
            }
            if (itemAction.Action == "buy")
            {
                if (item.Bid < item.Price / 2)
                {
                    return new ResponseObject(409, $"{user.Name}, your bit on the product {item.Name} is less then half ({item.Bid}) of the price ({item.Price}). You can not buy it. ");
                }
                else
                {
                    item.BoughtById = user.Id;
                    _db.SaveChanges();
                    return new ResponseObject(200, $"{user.Name}, you have bought item {item.Name} for {item.Bid} Dollars.");
                }
            }
            else // action = withdraw
            {
                user.Dollars += item.Bid;
                item.Bid = 0;
                item.BidById = 0;
                _db.SaveChanges();
                return new ResponseObject(200, $"{user.Name}, you have withdrawed the bit for {item.Name}");
            }
        }
    }
}
