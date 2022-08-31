﻿using GreenBay.Context;
using GreenBay.Models;
using GreenBay.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
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

        public ResponseLoginObjectDto Authenticate(UserLogin userLogin)
        {
            if (userLogin.UserName == null || userLogin.UserName == "")
            {
                return new ResponseLoginObjectDto(400, "Not valid username.");
            }
            if (userLogin.Password == null || userLogin.Password == "")
            {
                return new ResponseLoginObjectDto(400, "Not valid password.");
            }
            User currentUser = _db.Users.FirstOrDefault(u => u.Name.ToLower().Equals(userLogin.UserName.ToLower()));
            if  (currentUser == null)
            {
                return new ResponseLoginObjectDto(404, $"User with name {userLogin.UserName} not found in database.");
            }
            if(!currentUser.Password.Equals(userLogin.Password))
            {
                return new ResponseLoginObjectDto(409, "Given password is wrong.");
            }
            string token = GenerateToken(currentUser); 
            ResponseLoginObjectOutputDto output = new ResponseLoginObjectOutputDto(
                currentUser.Dollars, token);
            
            return new ResponseLoginObjectDto(200, "You have logged in.", output);
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
            if (EmailIsValid(userCreate.Email) == false)
            {
                return new ResponseObject(400, "No valid Email adress was given.");
            }
            if (_db.Users.Any(u => u.Email.Equals(userCreate.Email)))
            {
                return new ResponseObject(409, $"User with email {userCreate.Email} already exists.");
            }
            if (userCreate.Dollars < 0)
            {
                return new ResponseObject(400, "No valid dollars amount was given.");
            }
            if (_db.Users.Any(u => u.Name.ToLower().Equals(userCreate.UserName.ToLower()))) 
            {
                return new ResponseObject(409, $"User with name {userCreate.UserName} already exists.");
            }
            return new ResponseObject(201, $"User {userCreate.UserName} has been created.");
        }

        private bool EmailIsValid(string email)
        {
            var valid = true;
            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }
            return valid;
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

        public List<UserInfoDto> ListAllUsers(int page, int itemCount)
        {
            if (itemCount < 1) itemCount = 20;
            if (page < 1) page = 1;
            int totalCount = _db.Users.Count();
            if (totalCount < page * itemCount)
            {
                if (totalCount % itemCount == 0) page = totalCount / itemCount;
                else page = totalCount / itemCount + 1;
            }
            List<User> usersInDB = _db.Users.OrderByDescending(u => u.Id).Skip((page - 1) * itemCount).Take(itemCount).ToList();
            List<UserInfoDto> users = new List<UserInfoDto>();
            foreach (User user in usersInDB)
            {
                users.Add(new UserInfoDto( user.Id, user.Name, user.Password, user.Email
                    ,user.Dollars, user.Role, user.CreatedAt));
            }
            return users;
        }
    }
}
