using GreenBay.Context;
using System.Collections.Generic;

namespace GreenBay.Models
{
    public class Constants 
    {
        public static List<User> Users = new List<User>()
        { 
            new User() {Name = "Filip", Email = "filip.karlubik@gmail.com", Password = "f", Role = "admin", Dollars = 1000},
            new User() {Name = "Tom", Email = "thomas@gmail.com", Password = "t", Role = "user", Dollars = 100},
            new User() {Name = "Jerry", Email = "mouse@gmail.com", Password = "j", Role = "user", Dollars = 3000},
            new User() {Name = "Butch", Email = "possy_cat@gmail.com", Password = "b", Role = "user", Dollars = 50}
        };
        public static List<Item> Items = new List<Item>() {
            new Item() {Name = "Sausage", Description = "1kg, fresh good taste :)", UserId = 1, Price = 9
                , ImageUrl = "https://image.shutterstock.com/image-photo/grilled-bratwurst-pork-sausages-basil-260nw-1406560712.jpg" },
            new Item() {Name = "Cheese", Description = "200g Healthy delicious :D", UserId = 2, Price = 7
                , ImageUrl = "https://image.shutterstock.com/image-photo/piece-cheese-isolated-on-white-260nw-1416372146.jpg" }
        };
    }
}
