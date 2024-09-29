using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NewsApp.Core.Entities;
using System.Security.Claims;
using System.Text;

namespace NewsApp.Core.Services
{
    public sealed class AuthService(IConfiguration configuration)
    {
        public string CreateToken(User userRequest, string userRole)
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
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }

        // refresh token
    }
}
