using GreenBay.Context;
using GreenBay.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace GreenBay.Tests.IntegrationTests
{
    public class HTTPClientFactory
    {
        protected readonly HttpClient _client;
        protected readonly IConfiguration configuration;

        protected HTTPClientFactory()
        {
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build();   

            var appFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(ApplicationContext));
                    services.AddDbContext<ApplicationContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName: "TestingDatabase");
                    });
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var appDb = scopedServices.GetRequiredService<ApplicationContext>();
                        appDb.Database.EnsureDeleted();
                        appDb.Database.EnsureCreated();
                        appDb.Users.AddRange(Constants.Users);
                        appDb.SaveChanges();
                        appDb.Items.AddRange(Constants.Items);
                        appDb.SaveChanges();
                    }
                });
            });
            _client = appFactory.CreateClient();
            var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9naXZlbm5hbWUiOiJGaWxpcCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImZpbGlwLmtybHViaWtAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJleHAiOjE2NTk4MDExMDgsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzExLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzExLyJ9.wfDUN7ARNEEV0CQ4XGBBKlvyr2IMmjMWAhjCLeO_YIs";
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }
    }
}