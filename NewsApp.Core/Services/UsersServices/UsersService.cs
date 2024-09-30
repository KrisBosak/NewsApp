using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services.UsersServices.Interfaces;
using NewsApp.Core.Services.UsersServices.Models;

namespace NewsApp.Core.Services.UsersServices
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthService authService;

        public UsersService(UserManager<User> userManager, AuthService authService)
        {
            this._userManager = userManager;
            this.authService = authService;
        }

        public async Task<IdentityResult> UserSignUp(UserSignUpRequestModel request)
        {
            IdentityResult result = new();
            if (await _userManager.FindByEmailAsync(request.Email) == null)
            {
                var newUser = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    EmailConfirmed = true,
                    IsAuthor = request.IsAuthor,
                    IsActive = true

                };

                result = await _userManager.CreateAsync(newUser, request.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, request.IsAuthor
                        ? RolesEnum.Author.ToString()
                        : RolesEnum.User.ToString());
                }
            }

            return result;
        }

        public async Task<string> UserLogIn(UserLogInRequestModel request)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return string.Empty;
            }

            return authService.CreateToken(user, (await _userManager.GetRolesAsync(user)).First());
        }

        public async Task<IEnumerable<User>> GetAuthors()
        {
            return await _userManager.Users
                .Where(x => x.IsAuthor)
                .ToListAsync();
        }
    }
}
