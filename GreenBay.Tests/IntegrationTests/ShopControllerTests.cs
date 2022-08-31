using GreenBay.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GreenBay.Tests.IntegrationTests
{
    [Serializable]
    [Collection("Serialize")]
    public class ShopControllerTests : HTTPClientFactory
    {
        [Fact]
        public void Create_ValidInputShouldWork()
        {
            var response = _client.PostAsync("/shop/create"
                    , JsonContent.Create(new ItemCreate() { Name = "n", ImageUrl = "https://github.com/FilipKarlubik/GreenBay/pulls", Description = "d", Price = 1 })).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(201, (int)response.StatusCode);
            Assert.Equal("New item has been created", result["status"].ToString());
        }
        
        [Fact]
        public void Create_DuplicateNameShouldGetError()
        {
            var response = _client.PostAsync("/shop/create"
                    , JsonContent.Create(new ItemCreate() { Name = "Cheese", ImageUrl = "https://github.com/FilipKarlubik/GreenBay/pulls", Description = "d", Price = 1 })).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(409, (int)response.StatusCode);
            Assert.Equal("Item with name Cheese already exists.", result["error"].ToString());
        }

        [Fact]
        public void BidOnItem_BidOnOwnedItemShouldGetError()
        {
            var response = _client.PostAsync("/shop/bid"
                    , JsonContent.Create(new ItemBid() { ItemId = 1, Bid = 10 })).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(409, (int)response.StatusCode);
            Assert.Equal("Filip, you can not buy your own product!", result["error"].ToString());
        }

        [Fact]
        public void BidOnItem_BidOnValidItemShouldWork()
        {
            var response = _client.PostAsync("/shop/bid"
                    , JsonContent.Create(new ItemBid() { ItemId = 2, Bid = 10 })).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal("Filip, you have bought product Cheese for 10 dollars.", result["status"].ToString());
        }
    }
}
