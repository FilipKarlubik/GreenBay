using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace GreenBay.Services
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string emailAdressTo, string password, string login)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("eucyontribes@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emailAdressTo));
            email.Subject = "Sending Forgotten Credentials from GreenBay";
            email.Body = new TextPart(TextFormat.Html) { Text = $"Login = {login}, Password = {password}.\n" +
                $"<a href=\"https://greenbayapp.azurewebsites.net/login\">click here for login</a>" };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("eucyontribes@gmail.com", "fbyrnbfxarcjpjso");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
