using GreenBay.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenBay.Context
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Item> Items { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }
    }

}
