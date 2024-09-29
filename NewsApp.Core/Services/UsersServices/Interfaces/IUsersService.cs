using Microsoft.AspNetCore.Identity;
using NewsApp.Core.Services.UsersServices.Models;

namespace NewsApp.Core.Services.UsersServices.Interfaces
{
    public interface IUsersService
    {
        public Task<IdentityResult> UserSignUp(UserSignUpRequestModel request);
        public Task<string> UserLogIn(UserLogInRequestModel request);
    }
}
