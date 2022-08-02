using System;

namespace GreenBay.Models.DTOs
{
    public class ItemInfoDto
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int Price { get; }
        public DateTime CreatedAt { get; }
        public int HighestBid { get; }
        public string HighestBidBy { get; }
        public int HighestBidById { get; }
        public string ImageUrl { get; }
        public string SellingBy { get; }
        public int SellingById { get; }
        public string BoughtBy { get; }
        public int BoughtById { get; }

        public ItemInfoDto(int id, string name, string description, int price, DateTime createdAt, int highestBid, string highestBidBy, int highestBidById, string imageUrl, string sellingBy, int sellingById, string boughtBy, int boughtById)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CreatedAt = createdAt;
            HighestBid = highestBid;
            HighestBidBy = highestBidBy;
            HighestBidById = highestBidById;
            ImageUrl = imageUrl;
            SellingBy = sellingBy;
            SellingById = sellingById;
            BoughtBy = boughtBy;
            BoughtById = boughtById;
        }
    }
}
