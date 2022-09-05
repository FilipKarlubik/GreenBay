namespace GreenBay.Models
{
    public class ItemAction
    {
        public int Id { get; set; }
        public string Action { get; set; }

        public ItemAction(int id, string action)
        {
            Id = id;
            Action = action;
        }

        public ItemAction()
        {
        }
    }
}
