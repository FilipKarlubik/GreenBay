using System;
using System.Collections.Generic;

namespace GreenBay.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Dollars { get; set; }

        public List<Item> Items { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
