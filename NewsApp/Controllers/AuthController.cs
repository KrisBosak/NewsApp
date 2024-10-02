using Microsoft.AspNetCore.Mvc;
using NewsApp.Core.Services.Auth.Interfaces;
using NewsApp.Core.Services.Auth.Models;

namespace NewsApp.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("/signup")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpRequestModel request)
        {
            var result = await _authService.UserSignUp(request);

            if (result == null)
            {
                return BadRequest("User with entered email already exists.");
            }
            else if (result.Errors.Any())
            {
                return BadRequest(result.Errors.Select(x => x.Description));
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInRequestModel request)
        {
            var result = await _authService.UserLogIn(request);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("Incorrect email or password. Try again.");
            }

            return Ok(result);
        }
    }
}
