using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GreenBay.Tests.UnitTests
{
    [Serializable]
    [Collection("Serialize")]
    public class BuyServiceTests : IDisposable
    {
        private readonly ApplicationContext context;
        private readonly IBuyService buyService;
        private readonly ISellService sellService;
        private readonly static DbContextOptions options = new DbContextOptionsBuilder<ApplicationContext>()
               .UseInMemoryDatabase("BuyServiceTest").Options;

        public BuyServiceTests()
        {
            context = new ApplicationContext(options);
            sellService = new SellService(context);
            buyService = new BuyService(context, sellService);

            if (context.Users.Count() == 0)
            {
                context.Users.AddRange(Constants.Users);
                context.SaveChanges();
            }
            if (context.Items.Count() == 0)
            {
                context.Items.AddRange(Constants.Items);
                context.SaveChanges();
            }
        }
        public void Dispose()
        {
            foreach (var user in context.Users)
            {
                context.Users.Remove(user);
            }
            foreach (var item in context.Items)
            {
                context.Items.Remove(item);
            }
            context.SaveChanges();
        }

        [Theory]
        [InlineData(1, 1, 409, "Filip, you can not buy your own product!")]
        [InlineData(2, 1, 200, "Filip, you have risen the bit of item Cheese to 1 dollars.")]
        [InlineData(0, 1, 400, "Id:0 of item is not valid")]
        [InlineData(2, 1, 400, "Actual bid: 1 of product Cheese is higher or equal as yours: 1, you must higher your bid. Full price is 7.")]
        [InlineData(2, 7, 200, "Filip, you have bought item Cheese for 7 dollars.")]
        public void Bid_WithVariousParams(int itemIdid, int bid, int expectedStatusCode, string expectedMessage)
        {
            User user = context.Users.First(u => u.Name.Equals("Filip"));
            ItemBid itemBid = new ItemBid() {ItemId = itemIdid, Bid = bid };

            ResponseItemObjectDto result = buyService.Bid(itemBid, user);

            Assert.Equal(expectedStatusCode, result.StatusCode);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
