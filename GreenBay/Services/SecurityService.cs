using GreenBay.Context;
using GreenBay.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace GreenBay.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly ApplicationContext _db;
        private readonly IConfiguration _config;

        public SecurityService(ApplicationContext db, IConfiguration configuration)
        {
            _db = db;
            _config = configuration;
        }

        public User Authenticate(UserLogin userLogin)
        {
            var currentUser = _db.Users.FirstOrDefault(u => u.Name.ToLower().Equals(userLogin.UserName.ToLower())
            && u.Password.Equals(userLogin.Password));
            return currentUser;
        }

        public User CheckDuplicity(UserCreate userCreate)
        {
            if(userCreate == null) return null;
            if(userCreate.UserName == null || userCreate.UserName == String.Empty) return null;
            if(userCreate.Password == null || userCreate.Password == String.Empty) return null;
            if(userCreate.Email == null || userCreate.Email == String.Empty) return null;
            string role = "user";
            if (userCreate.Role.ToLower() == "admin") role = "admin";
            if (_db.Users.Any(u => u.Name.ToLower().Equals(userCreate.UserName.ToLower()))) return null;
            return new User()
            {
                Name = userCreate.UserName,
                Email = userCreate.Email,
                Password = userCreate.Password,
                Role = role
            };
        }

        public User DecodeUser(ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            return new User()
            {
                Name = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value,
                Email = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value,
                Role = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value,
                Id = Int32.Parse(userClaims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value)
            };
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
