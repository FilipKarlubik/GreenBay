using GreenBay.Context;
using GreenBay.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.IO;

namespace GreenBay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env != null && env.Equals("Development"))
            {
                Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .WriteTo.MSSqlServer(configuration.GetConnectionString("DefaultConnection"), autoCreateSqlTable: true, tableName: "Logs")
                .CreateLogger();
            }
            else
            {
                var connectionString = "Server=tcp:popescucql.database.windows.net,1433;Initial Catalog=PopescuDB;Persist Security Info=False;User ID=Filipescu1;Password=Popescu007;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .WriteTo.MSSqlServer(connectionString, autoCreateSqlTable: true, tableName: "Logs")
                .CreateLogger();
            }
            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();
                using (var serviceScope = host.Services.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
                    //context.Database.EnsureDeleted(); // uncomment if you want to restore with basic params
                    context.Database.EnsureCreated();
                    if (context.Users.Count() == 0) context.Users.AddRange(Constants.Users);
                    context.SaveChanges();
                    if (context.Items.Count() == 0) context.Items.AddRange(Constants.Items);
                    context.SaveChanges();
                }
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }  
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
