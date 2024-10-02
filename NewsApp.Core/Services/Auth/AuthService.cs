using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services.Auth.Interfaces;
using NewsApp.Core.Services.Auth.Models;
using System.Security.Claims;
using System.Text;

namespace NewsApp.Core.Services.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> _userManager;

        public AuthService(IConfiguration configuration, UserManager<User> userManager)
        {
            this.configuration = configuration;
            _userManager = userManager;
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

            return CreateToken(user, (await _userManager.GetRolesAsync(user)).First());
        }

        private string CreateToken(User userRequest, string userRole)
        {
            string key = configuration["Jwt:Key"] ?? string.Empty;
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, userRequest.Id),
                    new Claim(JwtRegisteredClaimNames.Email, userRequest.Email!),
                    new Claim("role", userRole)
                ]),
                Expires = DateTime.Now.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }

        // refresh token
    }
}
