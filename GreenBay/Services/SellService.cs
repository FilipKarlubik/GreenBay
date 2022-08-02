using GreenBay.Context;
using GreenBay.Models.DTOs;
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

        public List<ItemInfoDto> ListAllBuyableItems(int id)
        {
            int dollars = _db.Users.FirstOrDefault(u => u.Id.Equals(id)).Dollars;
            List<ItemInfoDto> items = ListAllItems();
            return items.Where(i => i.BoughtById.Equals(0) && i.HighestBid < dollars && i.SellingById != id).ToList();
        }

        public List<ItemInfoDto> ListAllSellableItems(int id)
        {
            int dollars = _db.Users.FirstOrDefault(u => u.Id.Equals(id)).Dollars;
            List<ItemInfoDto> items = ListAllItems();
            return items.Where(i => i.BoughtById.Equals(0)).ToList();
        }

        public List<ItemInfoDto> ListAllItems()
        {
            List<ItemInfoDto> items = new List<ItemInfoDto>();
            foreach (var item in _db.Items)
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
                items.Add(new ItemInfoDto(
                    item.Id, item.Name, item.Description, item.Price, item.CreatedAt, item.Bid
                    ,highestBidBy, item.BidById
                    ,item.ImageUrl, _db.Users.FirstOrDefault(u => u.Id.Equals(item.UserId)).Name,
                    _db.Users.FirstOrDefault(u => u.Id.Equals(item.UserId)).Id, boughtBy, item.BoughtById
                    ));
            }
            return items;
        }
    }
}
