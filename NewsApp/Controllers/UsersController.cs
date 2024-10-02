using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewsApp.Core.Entities;
using NewsApp.Core.Services.UsersServices.Interfaces;
using NewsApp.Core.Services.UsersServices.Models;
using NewsApp.Core.Services.Utils;

namespace NewsApp.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMemoryCache _cache;

        public UsersController(IUsersService usersService, IMemoryCache cache)
        {
            _usersService = usersService;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryHelpers)
        {
            PagedResult<User> result;
            var cacheKey = $"users-{queryHelpers.FilterBy}-{queryHelpers.FilterValue}-{queryHelpers.OrderBy}-{queryHelpers.OrderByDescending}-{queryHelpers.PageSize}-{queryHelpers.PageNumber}";

            if (!_cache.TryGetValue(cacheKey, out var cachedResult))
            {
                try
                {
                    result = await _usersService.GetAllUsers(queryHelpers);
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
                }

                if (result == null)
                {
                    return NotFound("There are no users.");
                }
                
                _cache.Set(cacheKey, result);

                return Ok(result);
            }

            return Ok(cachedResult);
        }

        [HttpGet]
        [Route("/authors")]
        public async Task<IActionResult> GetAuthors([FromQuery] QueryParameters queryHelpers)
        {
            PagedResult<User> result;
            var cacheKey = $"authors-{queryHelpers.FilterBy}-{queryHelpers.FilterValue}-{queryHelpers.OrderBy}-{queryHelpers.OrderByDescending}-{queryHelpers.PageSize}-{queryHelpers.PageNumber}";

            if (!_cache.TryGetValue(cacheKey, out var cachedResult))
            {
                try
                {
                    result = await _usersService.GetAuthors(queryHelpers);
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
                }                

                if (result == null)
                {
                    return NotFound("There are no authors.");
                }

                _cache.Set(cacheKey, result);

                return Ok(result);
            }

            return Ok(cachedResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] string id)
        {
            User? result;

            try
            {
                result = await _usersService.GetUser(id);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (result == null)
            {
                return NotFound("User does not exist.");
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserUpdateRequestModel request)
        {
            IdentityResult? result;

            try
            {
                result = await _usersService.UpdateUser(request);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (result == null)
            {
                return NotFound("User does not exist.");
            }
            else if (result.Errors.Any())
            {
                return BadRequest(result.Errors.Select(x => x.Description));
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeactivateOrDelete([FromRoute] string id, [FromQuery] bool permanentlyDelete)
        {
            bool result;

            try
            {
                result = await _usersService.DeactivateOrDeleteUser(id, permanentlyDelete);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (!result)
            {
                return NotFound("Invalid User. No action was taken.");
            }

            return Ok(permanentlyDelete 
                ? "User has been permanently deleted."
                : "User has been deactivated.");
        }

        // reset password 
        // God mode for admins -- blocking with a new flag on the user like isBlocked which would prevent the blocked user from liking, commenting, subscribing...
    }
}
