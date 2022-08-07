using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GreenBay.Tests.UnitTests
{
    [Serializable]
    [Collection("Serialize")]
    public class StoreServiceTests : IDisposable
    {
        private readonly IStoreService storeService;
        private readonly ISellService sellService;
        private readonly static DbContextOptions options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase("test").Options;
        private readonly ApplicationContext context;
        public StoreServiceTests()
        {
            context = new ApplicationContext(options);
            sellService = new SellService(context);
            storeService = new StoreService(context, sellService);    
            context.Database.EnsureCreated();
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

        [Fact]
        public async Task CreateUser_InputValidSchouldWork()
        {
            //Arrange     
            UserCreate userCreate = new UserCreate()
            { UserName = "Filip", Dollars = 100
            , Email = "filip.karlubik@gmail.com"
            , Role = "admin", Password = "f"};
            
            //Act
            storeService.CreateUser(userCreate);
            var user = await context.Users.FirstAsync();
            var usersCount = await context.Users.CountAsync();
            
                //Assert
            Assert.Equal("Filip", user.Name);
            Assert.Equal("filip.karlubik@gmail.com", user.Email);
            Assert.Equal("f", user.Password);
            Assert.Equal(100, user.Dollars);
            Assert.Equal("admin", user.Role);
            Assert.Equal(1, usersCount);
        }

        [Fact]
        public async Task CreateUser_RoleNotSpecifiedSchouldBeUser()
        {
            //Arrange     
            UserCreate userCreate = new UserCreate()
            {
                UserName = "Filip",
                Dollars = 100 ,
                Email = "filip.karlubik@gmail.com" ,
                Role = "none",
                Password = "f"
            };

            //Act
            storeService.CreateUser(userCreate);
            var user = await context.Users.FirstAsync();

            //Assert
            Assert.Equal("user", user.Role);
        }

        [Theory]
        [InlineData("Cheese", "1kg ", "https://www.google.com/", 1, 201)]
        [InlineData("Cheese", "", "https://www.google.com/", 1, 400)]
        [InlineData("", "1kg", "https://www.google.com/", 1, 400)]
        [InlineData("Cheese", "1kg", "not valid url", 1, 400)]
        [InlineData("Cheese", "1kg ", "https://www.google.com/", -1, 400)]
        [InlineData("", "", "", 1, 400)]
        public async Task CreateItem_VariousParams(string name, string description, string imageUrl
            , int price, int expextedStatusCode)
        {
            //Arrange     
            context.Users.Add(new User());
            context.SaveChanges();
            User user = await context.Users.FirstAsync();
            ItemCreate itemCreate = new ItemCreate()
            {
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                Price = price
            };
            //Act
            ResponseItemObjectDto result = storeService.CreateItem(itemCreate, user);
           
            //Assert
            Assert.Equal(expextedStatusCode, result.StatusCode);
        }

        [Fact]
        public async Task CreateItem_CreateWithDuplicateNameSchouldFail()
        {
            //Arrange     
            context.Users.Add(new User());
            context.SaveChanges();
            User user = await context.Users.FirstAsync();
            ItemCreate itemCreate = new ItemCreate()
            {
                Name = "X",
                Description = "X",
                ImageUrl = "https://www.google.com/",
                Price = 1
            };
            ItemCreate itemCreateDuplicName = new ItemCreate()
            {
                Name = "X",
                Description = "none",
                ImageUrl = "https://www.ggle.com/",
                Price = 5
            };
            //Act
            storeService.CreateItem(itemCreate, user);
            ResponseItemObjectDto result = storeService.CreateItem(itemCreateDuplicName, user);

            //Assert
            Assert.Equal(409, result.StatusCode);
        }

        [Fact]
        public void CreateItem_CreateWithNonExistingUserSchouldFail()
        {
            //Arrange       
            User user = new User();
            user.Id = 5;
            ItemCreate itemCreate = new ItemCreate()
            {
                Name = "X",
                Description = "X",
                ImageUrl = "https://www.google.com/",
                Price = 1
            };
           
            //Act
            ResponseItemObjectDto result = storeService.CreateItem(itemCreate, user);

            //Assert
            Assert.Equal(404, result.StatusCode);
        }

        [Theory]
        [InlineData("none", 1, 400)]
        [InlineData("increase", 1, 200)]
        [InlineData("decrease", 1, 200)]
        [InlineData("decrease", -1, 400)]
        [InlineData("decrease", 101, 409)]
        public void ManageMoney_VariousParams(string action,int amount, int expectedStatusCode)
        {
            //Arrange
            DollarsManage dollarsManage = new DollarsManage();
            dollarsManage.Action = action;
            dollarsManage.Amount = amount;
            context.Users.Add(new User() { 
            Dollars = 100});
            context.SaveChanges();
            User user = context.Users.First();

            //Act
            ResponseObject result = storeService.ManageMoney(dollarsManage, user.Id);

            //Assert
            Assert.Equal(expectedStatusCode, result.StatusCode);
        }
    }
}
