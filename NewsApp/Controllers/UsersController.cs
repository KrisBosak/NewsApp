using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services.UsersServices.Interfaces;
using NewsApp.Core.Services.UsersServices.Models;

namespace NewsApp.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        [Authorize(Policy = nameof(PoliciesEnum.God))]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpRequestModel request)
        {
            var result = await _usersService.UserSignUp(request);

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
            var result = await _usersService.UserLogIn(request);

            if(string.IsNullOrEmpty(result)) 
            {
                return BadRequest("Incorrect email or password. Try again.");
            }

            return Ok(result);
        }

        // reset password 
        // edit user
    }
}
