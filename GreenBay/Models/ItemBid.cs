namespace GreenBay.Models
{
    public class ItemBid
    {
        public int ItemId { get; set; }
        public int Bid { get; set; }

        public ItemBid(int itemId, int bid)
        {
            ItemId = itemId;
            Bid = bid;
        }

        public ItemBid()
        {
        }
    }
}
