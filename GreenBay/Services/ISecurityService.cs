using GreenBay.Models;
using GreenBay.Models.DTOs;
using System.Collections.Generic;
using System.Security.Claims;

namespace GreenBay.Services
{
    public interface ISecurityService
    {
        string GenerateToken(User user);
        ResponseLoginObjectDto Authenticate(UserLogin userLogin);
        ResponseObject CheckDuplicity(UserCreate userCreate);
        User DecodeUser(ClaimsIdentity claims);
        List<UserInfoDto> ListAllUsers(int page, int itemCount);
        ResponseObject ValidateCredentials(User user, Credentials credentials);
    }
}
