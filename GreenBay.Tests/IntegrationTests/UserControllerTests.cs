using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GreenBay.Context;
using System.Threading.Tasks;
using System.Text.Json;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Net.Http.Json;

namespace GreenBay.Tests.IntegrationTests
{
    [Serializable]
    [Collection("Serialize")]
    public class UserControllerTests : HTTPClientFactory
    {
        public UserControllerTests()
        {
        }

        [Fact]
        public void UserInfoAsync_SchouldReturnValidUserInfoOBject()
        {
            var response = _client.GetAsync("/user/info").Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal("Filip" , result["userName"].ToString());
            Assert.Equal("filip.karlubik@gmail.com", result["email"].ToString());
            Assert.Equal("admin", result["role"].ToString());
        }

        [Fact]
        public void Login_ValidSchouldReturnValidUserInfoOBject()
        {    
            var response = _client.PostAsync("/user/login"
                , JsonContent.Create(new UserLogin() {UserName = "Filip", Password = "f" })).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal("1000", result["dollars"].ToString());
        }

        [Fact]
        public void Login_InvalidPassSchouldReturnWrongPassError()
        {
            var response = _client.PostAsync("/user/login"
                , JsonContent.Create(new UserLogin() { UserName = "Filip", Password = "wrong pass" })).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(409, (int)response.StatusCode);
            Assert.Equal("Given password is wrong.", result["error"].ToString()); 
        }
    }
}
