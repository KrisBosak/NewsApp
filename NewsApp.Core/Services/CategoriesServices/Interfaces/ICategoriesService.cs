using NewsApp.Core.Entities;
using NewsApp.Core.Services.CategoriesServices.Models;

namespace NewsApp.Core.Services.CategoriesServices.Interfaces
{
    public interface ICategoriesService
    {
        public Task<Category?> GetCategory(int id);
        public Task<IEnumerable<Category>> GetAllCategories();
        public Task<Category> CreateCategory(CategoryCreateRequestModel request);
        public Task<Category?> UpdateCategory(CategoryUpdateRequestModel request);
        public Task<bool> DeleteCategory(int id);
    }
}
