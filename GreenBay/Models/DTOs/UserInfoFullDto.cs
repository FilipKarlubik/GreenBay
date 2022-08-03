using System;
using System.Collections.Generic;

namespace GreenBay.Models.DTOs
{
    public class UserInfoFullDto
    {
        public int Id { get; }
        public string UserName { get; }
        public string Password { get; }
        public string Email { get; }
        public int Dollars { get; }
        public string Role { get; }
        public DateTime CreatedAt { get; }
        public List<ItemInfoDto> ItemsSold { get; }
        public List<ItemInfoDto> ItemsBought { get; }
        public List<ItemInfoDto> ItemsTryingToBuy { get; }
        public List<ItemInfoDto> ItemsTryingToSell { get; }
       
        public UserInfoFullDto(int id, string userName, string password, string email, int dollars, string role, DateTime createdAt, List<ItemInfoDto> itemsSold, List<ItemInfoDto> itemsBought, List<ItemInfoDto> itemsTryingToBuy, List<ItemInfoDto> itemsTryingToSell)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Email = email;
            Dollars = dollars;
            Role = role;
            CreatedAt = createdAt;
            ItemsSold = itemsSold;
            ItemsBought = itemsBought;
            ItemsTryingToBuy = itemsTryingToBuy;
            ItemsTryingToSell = itemsTryingToSell;
        }
    }
}
