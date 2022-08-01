using GreenBay.Context;
using GreenBay.Models;

namespace GreenBay.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationContext _db;

        public StoreService(ApplicationContext db)
        {
            _db = db;
        }

        public void CreateUser(User user)
        {
           _db.Users.Add(user);
           _db.SaveChanges();
        }
    }
}
