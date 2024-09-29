using NewsApp.Core.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Services.CategoriesServices.Interfaces;
using NewsApp.Core.Services.CategoriesServices.Models;

namespace NewsApp.Core.Services.CategoriesServices
{
    public class CategoriesService : ICategoriesService
    {
        private readonly NewsDbContext _context;

        public CategoriesService(NewsDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CreateCategory(CategoryCreateRequestModel request)
        {
            Category category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            _context.Categories.Add(category);

            return await _context.SaveChangesAsync();
        }
    }
}
