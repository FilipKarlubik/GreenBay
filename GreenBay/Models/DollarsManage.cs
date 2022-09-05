namespace GreenBay.Models
{
    public class DollarsManage
    {
        public int Amount { get; set; }
        public string Action { get; set; }

        public DollarsManage(int amount, string action)
        {
            Amount = amount;
            Action = action;
        }

        public DollarsManage()
        {
        }
    }
}
