using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenBay.Services
{
    public class SellService : ISellService
    {
        private readonly ApplicationContext _db;

        public SellService(ApplicationContext db)
        {
            _db = db;
        }

        public List<ItemInfoDto> ListAllBuyableItems(int id, int page, int itemCount)
        {
            int dollars = _db.Users.FirstOrDefault(u => u.Id.Equals(id)).Dollars;
            List<ItemInfoDto> items = ListAllItems(1, Int32.MaxValue);
            items = items.Where(i => i.BoughtById.Equals(0) && i.HighestBid + 1 < dollars && i.SellingById != id).ToList();
            if (itemCount < 1)
            {
                itemCount = 20;
            }
            if (page < 1)
            {
                page = 1;
            }
            int totalCount = items.Count;
            if (totalCount < page * itemCount)
            {
                if (totalCount % itemCount == 0)
                {
                    page = totalCount / itemCount;
                }
                else
                {
                    page = totalCount / itemCount + 1;
                }
            }
            return items
                .OrderByDescending(u => u.Id).Skip((page - 1) * itemCount)
                .Take(itemCount).ToList();
        }

        public List<ItemInfoDto> ListAllSellableItems(int id, int page, int itemCount)
        {
            int dollars = _db.Users.FirstOrDefault(u => u.Id.Equals(id)).Dollars;
            List<ItemInfoDto> items = ListAllItems(1, Int32.MaxValue);
            items = items.Where(i => i.BoughtById.Equals(0)).ToList();
            if (itemCount < 1)
            {
                itemCount = 20;
            }
            if (page < 1)
            {
                page = 1;
            }
            int totalCount = items.Count;
            if (totalCount < page * itemCount)
            {
                if (totalCount % itemCount == 0)
                {
                    page = totalCount / itemCount;
                }
                else
                {
                    page = totalCount / itemCount + 1;
                }
            }
            return items
                .OrderByDescending(u => u.Id).Skip((page - 1) * itemCount)
                .Take(itemCount).ToList();
        }

        public List<ItemInfoDto> ListAllItems(int page, int itemCount)
        {
            if (itemCount < 1)
            {
                itemCount = 20;
            }
            if (page < 1)
            {
                page = 1;
            }
            int totalCount = _db.Items.Count();
            if (totalCount < page * itemCount)
            {
                if (totalCount % itemCount == 0)
                {
                    page = totalCount / itemCount;
                }
                else
                {
                    page = totalCount / itemCount + 1;
                }
            }
            List<Item> itemsInDb = _db.Items.OrderByDescending(u => u.Id).Skip((page - 1) * itemCount).Take(itemCount).ToList();
            List<ItemInfoDto> items = new();
            foreach (var item in itemsInDb)
            {  
                items.Add(GenerateItemInfo(item));
            }
            return items;
        }

        public UserInfoFullDto UserInfoDetailed(int id, string token)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            List<ItemInfoDto> items = ListAllItems(1, Int32.MaxValue);
            if (user == null) return null;
            return new UserInfoFullDto(user.Id, user.Name, user.Password,user.Email, user.Dollars
                , user.Role, user.CreatedAt
                , items.Where(i => i.BoughtById > 0 && i.SellingById.Equals(user.Id)).ToList()
                , items.Where(i => i.BoughtById.Equals(user.Id)).ToList()
                , items.Where(i => i.BoughtById == 0 && i.HighestBidById.Equals(user.Id)).ToList()
                , items.Where(i => i.BoughtById == 0 && i.SellingById.Equals(user.Id)).ToList()
                , token);
        }

        public ResponseItemObjectDto ItemInfo(int id)
        {
            if (id < 1)
            {
                return new ResponseItemObjectDto(400, $"Not valid item Id:{id} was given.");
            }
            Item item = _db.Items.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return new ResponseItemObjectDto(404, $"Item Id:{id} not in database.");
            }
            return new ResponseItemObjectDto(200, $"Item with Id:{id} has been found.", GenerateItemInfo(item));
        }

        public ItemInfoDto GenerateItemInfo(Item item)
        {
            string boughtBy = "none";
            if (_db.Users.Any(u => u.Id.Equals(item.BoughtById)))
            {
                boughtBy = _db.Users.FirstOrDefault(u => u.Id.Equals(item.BoughtById)).Name;
            }
            string highestBidBy = "none";
            if (_db.Users.Any(u => u.Id.Equals(item.BidById)))
            {
                highestBidBy = _db.Users.FirstOrDefault(u => u.Id.Equals(item.BidById)).Name;
            }
            return new ItemInfoDto(
                    item.Id, item.Name, item.Description, item.Price, item.CreatedAt, item.Bid
                    , highestBidBy, item.BidById
                    , item.ImageUrl, _db.Users.FirstOrDefault(u => u.Id.Equals(item.UserId)).Name,
                    _db.Users.FirstOrDefault(u => u.Id.Equals(item.UserId)).Id, boughtBy, item.BoughtById
                    );
        }
    }
}
