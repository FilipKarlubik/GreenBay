using GreenBay.Context;
using System.Collections.Generic;

namespace GreenBay.Models
{
    public class Constants 
    {
        public static List<User> Users = new List<User>()
        { 
            new User() {Name = "Filip", Email = "filip.krlubik@gmail.com", Password = "f", Role = "admin"},
            new User() {Name = "Tom", Email = "thomas@gmail.com", Password = "t", Role = "user"},
            new User() {Name = "Jerry", Email = "mouse@gmail.com", Password = "j", Role = "user"},
            new User() {Name = "Butch", Email = "possy_cat@gmail.com", Password = "b", Role = "user"}
        };
        public static List<Item> Items = new List<Item>() { 
            new Item() {Name = "Sausage", Description = "1kg, fresh good taste :)", UserId = 1
                , ImageUrl = "https://image.shutterstock.com/image-photo/grilled-bratwurst-pork-sausages-basil-260nw-1406560712.jpg" },
            new Item() {Name = "Cheese", Description = "200g Healthy delicious :D", UserId = 2
                , ImageUrl = "https://image.shutterstock.com/image-photo/piece-cheese-isolated-on-white-260nw-1416372146.jpg" }
        };
    }
}
