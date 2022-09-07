using System;
using System.Collections.Generic;
using System.Text;

namespace GreenBay.Models
{
    public static class Constants 
    {
        private const string Key = "superSecureSecredKey!";
        
        public static readonly List<User> Users = new()
        { 
            new User() {Name = "Filip", Email = "filip.karlubik@gmail.com", Password = "f", Role = "admin", Dollars = 1000},
            new User() {Name = "Tom", Email = "thomas@gmail.com", Password = "t", Role = "user", Dollars = 100},
            new User() {Name = "Jerry", Email = "mouse@gmail.com", Password = "j", Role = "user", Dollars = 3000},
            new User() {Name = "Butch", Email = "possy_cat@gmail.com", Password = "b", Role = "user", Dollars = 50}
        };
        
        public static  readonly List<Item> Items = new() {
            new Item() {Name = "Sausage", Description = "1kg, fresh good taste :)", UserId = 1, Price = 9
                , ImageUrl = "https://image.shutterstock.com/image-photo/grilled-bratwurst-pork-sausages-basil-260nw-1406560712.jpg" },
            new Item() {Name = "Cheese", Description = "200g Healthy delicious :D", UserId = 2, Price = 7
                , ImageUrl = "https://image.shutterstock.com/image-photo/piece-cheese-isolated-on-white-260nw-1416372146.jpg" }
        };
        
        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            password += Key;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passwordBytes);
        }
        
        public static string DecryptPassword(string base64EncodeData)
        {
            if (string.IsNullOrEmpty(base64EncodeData)) return "";
            var base64EncodeBytes = Convert.FromBase64String(base64EncodeData);
            var result = Encoding.UTF8.GetString(base64EncodeBytes);
            return result[..^Key.Length];
        }
    }
}
