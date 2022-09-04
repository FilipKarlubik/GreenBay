namespace GreenBay.Models
{
    public class ItemCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }

        public ItemCreate(string name, string description, string imageUrl, int price)
        {
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Price = price;
        }

        public ItemCreate()
        {
        }
    }
}
