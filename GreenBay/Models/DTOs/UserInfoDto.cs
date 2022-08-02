using System;

namespace GreenBay.Models.DTOs
{
    public class UserInfoDto
    {
        public int Id { get; }
        public string UserName { get; }
        public string Password { get; }
        public string Email { get; }
        public int Dollars { get; }
        public string Role { get; }
        public DateTime CreatedAt { get; }

        public UserInfoDto(int id, string userName, string password, string email, int dollars, string role, DateTime createdAt)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Email = email;
            Dollars = dollars;
            Role = role;
            CreatedAt = createdAt;
        }
    }
}
