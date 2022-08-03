namespace GreenBay.Models.DTOs
{
    public class ResponseItemObjectDto
    {
        public int StatusCode { get; }
        public string Message { get; }
        public ItemInfoDto ItemInfo { get; }

        public ResponseItemObjectDto(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ResponseItemObjectDto(int statusCode, string message, ItemInfoDto itemInfo) : this(statusCode, message)
        {
            ItemInfo = itemInfo;
        }
    }
}
