namespace GreenBay.Models.DTOs
{
    public class ResponseLoginObjectOutputDto
    {
        public int Dollars { get; }
        public string Token { get; }

        public ResponseLoginObjectOutputDto(int dollars, string token)
        {
            Dollars = dollars;
            Token = token;
        }
    }
}
