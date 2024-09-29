using NewsApp.Core.Services.CategoriesServices.Models;

namespace NewsApp.Core.Services.CategoriesServices.Interfaces
{
    public interface ICategoriesService
    {
        public Task<int> CreateCategory(CategoryCreateRequestModel request);
    }
}
