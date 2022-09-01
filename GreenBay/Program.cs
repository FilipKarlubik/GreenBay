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
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using GreenBay.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog.Core;
using Constants = GreenBay.Models.Constants;
using System.Configuration;

namespace GreenBay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");

            if (env != null && env.Equals("Development"))
            {
                connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
                builder.Services.AddDbContext<ApplicationContext>(dbBuilder => dbBuilder.UseSqlServer(connectionString));
            }
            else if (env != null && env.Equals("Production"))
            {
                connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
                builder.Services.AddDbContext<ApplicationContext>(dbBuilder => dbBuilder.UseSqlServer(connectionString));
            }
     
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(connectionString, autoCreateSqlTable: true, tableName: "Logs")
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                     };
                 });
            builder.Services.AddMvc();
            builder.Services.AddControllers();
            builder.Services.AddRazorPages();

            builder.Services.AddTransient<ISecurityService, SecurityService>();
            builder.Services.AddTransient<IStoreService, StoreService>();
            builder.Services.AddTransient<ISellService, SellService>();
            builder.Services.AddTransient<IBuyService, BuyService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            if (app.Environment.IsDevelopment())
            {
                try
                {
                    Log.Information("Starting up");
                    using (var serviceScope = app.Services.CreateScope())
                    {
                        var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
                        //context.Database.EnsureDeleted(); // uncomment if you want to restore with basic params
                        context.Database.EnsureCreated();
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
                    app.Run();
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
            else app.Run();
        }     
    }
}
