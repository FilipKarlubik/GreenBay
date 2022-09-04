namespace GreenBay.Models
{
    public class Credentials
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public Credentials(string email)
        {
            Email = email;
        }

        public Credentials()
        {
        }
    }
}
