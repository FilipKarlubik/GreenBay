﻿namespace GreenBay.Models
{
    public class UserCreate
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int Dollars { get; set; }
    }
}
