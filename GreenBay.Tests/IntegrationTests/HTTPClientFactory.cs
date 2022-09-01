using GreenBay.Context;
using GreenBay.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Net.Http;

namespace GreenBay.Tests.IntegrationTests
{
    public class HTTPClientFactory
    {
        protected readonly HttpClient _client;
        protected HTTPClientFactory()
        { 
            var appFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
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
                        //appDb.Database.EnsureDeleted();
                        //appDb.Database.EnsureCreated();
                        if (appDb.Users.Count() == 0)
                        {
                            appDb.Users.AddRange(Constants.Users);
                            appDb.SaveChanges();
                        }
                        if (appDb.Items.Count() == 0)
                        {
                            appDb.Items.AddRange(Constants.Items);
                            appDb.SaveChanges();
                        }
                    }
                });
            });
            _client = appFactory.CreateClient();
            var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9naXZlbm5hbWUiOiJGaWxpcCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImZpbGlwLmtybHViaWtAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJleHAiOjE5NzU0MjU4ODMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzExLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzExLyJ9.0nNQ6UXV_KHvidPkgr8yvat4BDVKm3ex3JxAdjO_i1Q";
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }
    }
}