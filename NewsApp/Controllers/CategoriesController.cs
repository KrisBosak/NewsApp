using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;
using NewsApp.Core.Services.CategoriesServices.Interfaces;
using NewsApp.Core.Services.CategoriesServices.Models;

namespace NewsApp.Api.Controllers
{
    [ApiController]
    [Route("categories")]
    [Authorize(Roles = nameof(RolesEnum.Admin))]
    public class CategoriesController : ControllerBase
    {
        const int NAME_MAX_LENGTH = 25;
        const int NAME_MIN_LENGTH = 2;
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            Category? result;

            try
            {
                result = await _categoriesService.GetCategory(id);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (result == null) return NotFound("No matching category.");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Category> result;

            try
            {
                result = await _categoriesService.GetAllCategories();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (!result.Any()) return NotFound("There are no categories."); 

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryCreateRequestModel request)
        {
            Category result;

            if (request.Name.Length > NAME_MAX_LENGTH || request.Name.Length < NAME_MIN_LENGTH) return BadRequest($"Name can't have more than {NAME_MAX_LENGTH} characters or less than {NAME_MIN_LENGTH}.");

            try
            {
                result = await _categoriesService.CreateCategory(request);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateRequestModel request)
        {
            Category? result;

            if (request.Name.Length > NAME_MAX_LENGTH || request.Name.Length < NAME_MIN_LENGTH) return BadRequest($"Name can't have more than {NAME_MAX_LENGTH} characters or less than {NAME_MIN_LENGTH}.");

            try
            {
                result = await _categoriesService.UpdateCategory(request);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }
            
            if (result == null) return NotFound("Invalid Category. No action was taken.");

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool result;
            
            try
            {
                result = await _categoriesService.DeleteCategory(id);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request. " + e });
            }

            if (!result) return NotFound("Invalid Category. No action was taken.");            

            return Ok("Category has been deleted.");
        }
    }
}
