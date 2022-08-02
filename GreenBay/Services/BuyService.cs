using GreenBay.Context;
using GreenBay.Models;
using System.Linq;

namespace GreenBay.Services
{
    public class BuyService : IBuyService
    {
        private readonly ApplicationContext _db;

        public BuyService(ApplicationContext db)
        {
            _db = db;
        }

        public ResponseObject Bid(ItemBid item, User user)
        {
            if (item == null)
            {
                return new ResponseObject(400, "Not valid input object");
            }
            if (item.ItemId < 1)
            {
                return new ResponseObject(400, $"Id:{item.ItemId} of item is not valid");
            }
            Item itemToBid = _db.Items.FirstOrDefault(i => i.Id.Equals(item.ItemId));
            if (itemToBid == null)
            {
                return new ResponseObject(404, $"Item with id:{item.ItemId} is not in database");
            } 
            if (itemToBid.BoughtById > 0)
            {
                string boughtBy = _db.Users.FirstOrDefault(u => u.Id.Equals(itemToBid.BoughtById)).Name;
                return new ResponseObject(404, $"Item with id:{item.ItemId} is already bought by {boughtBy}.");
            }
            if (itemToBid.Bid >= item.Bid)
            {
                return new ResponseObject(400, $"Actual bid {itemToBid.Bid} is higher or equal as yours, you must higher your bid.");
            }
            user = _db.Users.FirstOrDefault(u => u.Id.Equals(user.Id));
            if (user == null)
            {
                return new ResponseObject(404, "User is not in database");
            }
            if (itemToBid.BidById.Equals(user.Id))
            { 
                if(user.Dollars < item.Bid - itemToBid.Bid)
                {
                    return new ResponseObject(404, "You have not enough money to rise the bid");
                }
                else
                {
                    user.Dollars = item.Bid - itemToBid.Bid;
                    itemToBid.Bid = item.Bid;
                    if (itemToBid.Bid >= itemToBid.Price)
                    {
                        itemToBid.BoughtById = user.Id;
                        User giveMoneyForSellToUser = _db.Users.FirstOrDefault(u => u.Id.Equals(itemToBid.UserId));
                        giveMoneyForSellToUser.Dollars += itemToBid.Bid;
                        _db.SaveChanges();
                        return new ResponseObject(200, $"You have bought item {itemToBid.Name} for {itemToBid.Bid} dollars.");
                    }
                    else
                    {
                        _db.SaveChanges();
                        return new ResponseObject(200, $"You have risen the bit of item {itemToBid.Name} to {itemToBid.Bid} dollars.");
                    }
                }
            }
            else
            {
                if(itemToBid.Bid > 0)
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
                    return new ResponseObject(200, $"You have bought product {itemToBid.Name} for {itemToBid.Bid} dollars.");
                }
                else
                {
                    _db.SaveChanges();
                    return new ResponseObject(200, $"You have risen the bit of item {itemToBid.Name} to {itemToBid.Bid} dollars.");
                }
            }
        }
    }
}
