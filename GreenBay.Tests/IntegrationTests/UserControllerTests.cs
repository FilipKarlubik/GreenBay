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
using System.Text.Json;
using System.Threading.Tasks;

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
        public async Task UserInfoAsync_SchouldReturnValidUserInfoOBject()
        {
            var response = await _client.GetAsync("/user/info");
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal("Filip" , result["userName"].ToString());
            Assert.Equal("filip.karlubik@gmail.com", result["email"].ToString());
            Assert.Equal("admin", result["role"].ToString());
        }
    }
}
