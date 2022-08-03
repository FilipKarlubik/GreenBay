namespace GreenBay.Models.DTOs
{
    public class ResponseLoginObjectDto
    {
        public int StatusCode { get; }
        public string Message { get; }
        public ResponseLoginObjectOutputDto ResponseLoginObjectOutput { get; }

        public ResponseLoginObjectDto(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ResponseLoginObjectDto(int statusCode, string message, ResponseLoginObjectOutputDto responseLoginObjectOutput) : this(statusCode, message)
        {
            ResponseLoginObjectOutput = responseLoginObjectOutput;
        }
    }
}
