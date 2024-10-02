using Microsoft.EntityFrameworkCore;
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

        public async Task<Category?> GetCategory(int id) 
        {
            return await _context.Categories
                .Include(x => x.Articles)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories
                .Include(x => x.Articles)
                .ToListAsync();
        }

        public async Task<Category> CreateCategory(CategoryCreateRequestModel request)
        {
            Category category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> UpdateCategory(CategoryUpdateRequestModel request)
        {
            var categoryToUpdate = await _context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (categoryToUpdate == null) return null;

            categoryToUpdate.Name = request.Name;
            categoryToUpdate.Description = request.Description;

            _context.Categories.Update(categoryToUpdate);
            await _context.SaveChangesAsync();

            return categoryToUpdate;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var categoryToDelete= await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (categoryToDelete == null) return false;

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
