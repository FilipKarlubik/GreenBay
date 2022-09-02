using System.Threading.Tasks;

namespace GreenBay.Services
{
    public interface IEmailService
    {
        void SendEmail(string email, string password, string login);
    }
}