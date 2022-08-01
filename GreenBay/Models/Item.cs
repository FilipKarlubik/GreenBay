using System;

namespace GreenBay.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public bool Sold { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Item()
        {
            CreatedAt = DateTime.Now;
            Sold = false;
        }
    }
}
