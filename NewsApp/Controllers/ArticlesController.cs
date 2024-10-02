using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services.ArticlesServices.Interfaces;
using NewsApp.Core.Services.ArticlesServices.Models;
using NewsApp.Core.Services.Utils;

namespace NewsApp.Api.Controllers
{
    [ApiController]
    [Route("articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesService _articlesService;
        private readonly UserManager<User> _userManager;
        private readonly IMemoryCache _cache;

        public ArticlesController(IArticlesService articlesService, UserManager<User> userManager, IMemoryCache cache)
        {
            _articlesService = articlesService;
            _userManager = userManager;
            _cache = cache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            Article? result;

            try
            {
                result = await _articlesService.GetArticle(id);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (result == null) return NotFound("Article does not exist.");            

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryHelpers)
        {
            PagedResult<Article> result; 
            var cacheKey = $"articles-{queryHelpers.FilterBy}-{queryHelpers.FilterValue}-{queryHelpers.OrderBy}-{queryHelpers.OrderByDescending}-{queryHelpers.PageSize}-{queryHelpers.PageNumber}";

            if (!_cache.TryGetValue(cacheKey, out var cachedResult))
            {
                try
                {
                    result = await _articlesService.GetArticles(queryHelpers);
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
                }

                if (!result.Items.Any()) return NotFound("There are no articles.");
                
                _cache.Set(cacheKey, result);

                return Ok(result);
            }

            return Ok(cachedResult);
        }
        [HttpPost]
        [Authorize(Roles = $"{nameof(RolesEnum.Author)},{nameof(RolesEnum.Admin)}")]
        public async Task<IActionResult> Create([FromForm] ArticleCreateRequestModel requestModel)
        {
            User? user;
            Article? result;

            try
            {
                // I wasn't exactly sure if we will be getting a user id from the client side so i decided to try this out. In other places, I assume we will get the id's from the client side.
                user = await _userManager.GetUserAsync(User);
            }
            catch (Exception e)
            {
                if (!string.IsNullOrWhiteSpace(e.Message)) return BadRequest(e.Message);

                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
            try
            {
                result = await _articlesService.CreateArticle(requestModel, user!);
            }
            catch (Exception e)
            {
                if (!string.IsNullOrWhiteSpace(e.Message)) return BadRequest(e.Message);

                return StatusCode(500, new { Message = "An error occurred while processing your request."});
            }
            if (user == null) return NotFound("Current user can not be found. Try to log in again.");
            if (result == null) return BadRequest("Article couldn't be created. Please try again");

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = $"{nameof(RolesEnum.Author)},{nameof(RolesEnum.Admin)}")]
        public async Task<IActionResult> Update([FromForm] ArticleUpdateRequestModel requestModel)
        {
            Article? result;

            try
            {
                result = await _articlesService.UpdateArticle(requestModel);
            }
            catch (Exception e)
            {
                if (!string.IsNullOrWhiteSpace(e.Message)) return BadRequest(e.Message);

                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }

            if (result == null) return NotFound("Provided article couldn't be updated.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(RolesEnum.Author)},{nameof(RolesEnum.Admin)}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            bool result;

            try
            {
                result = await _articlesService.DeleteArticle(id);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e});
            }

            if (!result)
            {
                return NotFound("Invalid article. No action was taken.");
            }

            return Ok(result);
        }

        // get "for you" page with subscriber authors and categories
    }
}
