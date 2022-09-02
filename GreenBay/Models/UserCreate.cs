namespace GreenBay.Models
{
    public class UserCreate
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int Dollars { get; set; }

        public UserCreate(string userName, string email, string password, string role, int dollars)
        {
            UserName = userName;
            Email = email;
            Password = password;
            Role = role;
            Dollars = dollars;
        }

        public UserCreate()
        {
        }
    }
}
