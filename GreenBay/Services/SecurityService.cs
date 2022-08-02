using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

        public ResponseObject Authenticate(UserLogin userLogin)
        {
            if (userLogin.UserName == null || userLogin.UserName == "")
            {
                return new ResponseObject(400, "Not valid username.");
            }
            if (userLogin.Password == null || userLogin.Password == "")
            {
                return new ResponseObject(400, "Not valid password.");
            }
            User currentUser = _db.Users.FirstOrDefault(u => u.Name.ToLower().Equals(userLogin.UserName.ToLower())
            && u.Password.Equals(userLogin.Password));
            if  (currentUser == null)
            {
                return new ResponseObject(404, "User not found in database.");
            }
            string token = GenerateToken(currentUser);
            return new ResponseObject(200, token);
        }

        public ResponseObject CheckDuplicity(UserCreate userCreate)
        {
            // returns null if user is in database
            if (userCreate == null)
            {
                return new ResponseObject(400, "Not valid input object.");
            }
            if(userCreate.UserName == null || userCreate.UserName == String.Empty)
            {
                return new ResponseObject(400, "No UserName was given.");
            }
            if (userCreate.Password == null || userCreate.Password == String.Empty)
            {
                return new ResponseObject(400, "No password was given.");
            }
            if (userCreate.Email == null || userCreate.Email == String.Empty) 
            {
                return new ResponseObject(400, "No Email adress was given.");
            }
            if (_db.Users.Any(u => u.Name.ToLower().Equals(userCreate.UserName.ToLower()))) 
            {
                return new ResponseObject(409, $"User with name {userCreate.UserName} already exists.");
            }
            return new ResponseObject(200, $"User {userCreate.UserName} has been created.");
        }

        public User DecodeUser(ClaimsIdentity identity)
        {
            // returns user with params stored in token
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

        public List<UserInfoDto> ListAllUsers()
        {
            List<UserInfoDto> users = new List<UserInfoDto>();
            foreach (User user in _db.Users.ToList())
            {
                users.Add(new UserInfoDto( user.Id, user.Name, user.Password, user.Email
                    ,user.Dollars, user.Role, user.CreatedAt));
            }
            return users;
        }
    }
}
