using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services.ArticlesServices.Interfaces;
using NewsApp.Core.Services.ArticlesServices.Models;

namespace NewsApp.Api.Controllers
{
    [ApiController]
    [Route("articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesService _articlesService;
        private readonly UserManager<User> _userManager;

        public ArticlesController(IArticlesService articlesService, UserManager<User> userManager)
        {
            _articlesService = articlesService;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] int id)
        {
            var result = await _articlesService.GetArticle(id);

            if (result == null)
            {
                return NotFound("Article does not exist.");
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _articlesService.GetArticles();

            if (result == null)
            {
                return NotFound("There are no articles.");
            }

            return Ok(result);
        }

        // get for you

        [HttpPost]
        [Authorize(Roles = $"{nameof(RolesEnum.Author)},{nameof(RolesEnum.Admin)}")]
        public async Task<IActionResult> Create([FromForm] ArticleCreateRequestModel requestModel)
        {
            // needs more validations
            var user = await _userManager.GetUserAsync(User);
            var result = await _articlesService.CreateArticle(requestModel, user!);

            return Ok(result);
        }


        // edit
        [HttpDelete]
        [Authorize(Roles = $"{nameof(RolesEnum.Author)},{nameof(RolesEnum.Admin)}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _articlesService.DeleteArticle(id);

            if (!result)
            {
                return NotFound("Invalid article. No action was taken.");
            }

            return Ok(result);
        }
    }
}
