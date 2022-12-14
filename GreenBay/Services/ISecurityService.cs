using GreenBay.Models;
using GreenBay.Models.DTOs;
using Microsoft.AspNetCore.Http;
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
        User GetUserFromDB(int id);
        int CheckJWTCookieValidityReturnsUserID(IRequestCookieCollection cookies);
        int ValidateToken(string token);
        ResponseObject EncryptPasswords();
        int ReadPageFromCookies(IRequestCookieCollection cookies);
        int ReadItemCountFromCookies(IRequestCookieCollection cookies);
        string ReadSortByFromCookies(IRequestCookieCollection cookies);
    }
}
