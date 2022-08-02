namespace GreenBay.Models
{
    public class ResponseObject
    {
        public int StatusCode { get; }
        public string Message { get; }

        public ResponseObject(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
