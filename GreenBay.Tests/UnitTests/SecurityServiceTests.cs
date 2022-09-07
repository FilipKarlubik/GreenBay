using Castle.Core.Configuration;
using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using GreenBay.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace GreenBay.Tests.UnitTests
{
    [Serializable]
    [Collection("Serialize")]
    public class SecurityServiceTests : IDisposable
    {
        private readonly ApplicationContext context;
        private readonly ISecurityService securityService;
        private readonly IEmailService emailService;
        private readonly static DbContextOptions options = new DbContextOptionsBuilder<ApplicationContext>()
               .UseInMemoryDatabase("SecurityServiceTestDB").Options;

        public SecurityServiceTests()
        {

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build(); 
            context = new ApplicationContext(options);
            emailService = new EmailService();
            securityService = new SecurityService(context, config, emailService);

            context.Users.Add(new User() {Name = "Filip", Password = "ZnN1cGVyU2VjdXJlU2VjcmVkS2V5IQ=="
                , Email = "filip.karlubik@gmail.com", Role = "Admin", Dollars = 100 });
            context.SaveChanges();
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
        [InlineData("Filip", "f", 200, "You have logged in.")]
        [InlineData("Filip", "", 400, "Not valid password.")]
        [InlineData("", "f", 400, "Not valid username.")]
        [InlineData("Filip", "wrong password", 409, "Given password is wrong.")]
        [InlineData("Adam", "a", 404, "User with name Adam not found in database.")]
        public void Authenticate_VariousParams(string userName, string password
            , int expectedStatusCode, string expectedMessage)
        {
            //Arrange
            UserLogin userlogin = new() { UserName = userName, Password = password};
            
            //Act
            ResponseLoginObjectDto result = securityService.Authenticate(userlogin);

            //Assert
            Assert.Equal(expectedStatusCode, result.StatusCode);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Theory]
        [InlineData("Filip", "f","f.ka@gmail.com", 1,  409, "User with name Filip already exists.")]
        [InlineData("Lacko", "l","l.l@gmail.com", 1,  201, "User Lacko has been created.")]
        [InlineData("Lacko", "","l.l@gmail.com", 1,  400, "No password was given.")]
        [InlineData("", "lacko","l.l@gmail.com", 1,  400, "No UserName was given.")]
        [InlineData("Lacko", "lacko","", 1,  400, "No Email adress was given.")]
        [InlineData("Lacko", "lacko","not valid email", 1,  400, "No valid Email adress was given.")]
        [InlineData("Lacko", "lacko", "l.l@gmail.com", -1,  400, "No valid dollars amount was given.")]
    
        public void CheckDuplicity_VariousParams(string userName, string password
            , string email, int dollars, int expectedStatusCode, string expectedMessage)
        {
            //Arrange
            UserCreate userCreate = new() { UserName = userName, Password = password 
                , Email = email, Dollars = dollars};

            //Act
            ResponseObject result = securityService.CheckDuplicity(userCreate);

            //Assert
            Assert.Equal(expectedStatusCode, result.StatusCode);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public void GenerateToken_ValidUserSchouldWork()
        {
            //Arrange
            User user = context.Users.First();

            //Act
            string tokenString = securityService.GenerateToken(user);
            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            var email = token.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            //Assert
            Assert.NotNull(token);
            Assert.Equal("filip.karlubik@gmail.com", email);
        }

        [Fact]
        public void ListAllUsers_ShouldWorkOneUSerInDB()
        { 
            Assert.Single(securityService.ListAllUsers(1,10));
        }
    }
}
