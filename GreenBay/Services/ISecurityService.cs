using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Collections.Generic;
using System.Security.Claims;

namespace GreenBay.Services
{
    public interface ISecurityService
    {
        string GenerateToken(User user);
        User Authenticate(UserLogin userLogin);
        User CheckDuplicity(UserCreate userCreate);
        User DecodeUser(ClaimsIdentity claims);
        List<UserInfoDto> ListAllUsers();
    }
}
