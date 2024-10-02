using Microsoft.AspNetCore.Identity;
using NewsApp.Core.Services.Auth.Models;

namespace NewsApp.Core.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        public Task<IdentityResult> UserSignUp(UserSignUpRequestModel request);
        public Task<string> UserLogIn(UserLogInRequestModel request);
    }
}
