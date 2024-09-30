using NewsApp.Core.Entities;
using NewsApp.Core.Services.ArticlesServices.Models;

namespace NewsApp.Core.Services.ArticlesServices.Interfaces
{
    public interface IArticlesService
    {
        public Task<Article?> GetArticle(int id);
        public Task<IEnumerable<Article>> GetArticles(); 
        public Task<int> CreateArticle(ArticleCreateRequestModel requestModel, User user);
        public Task<bool> DeleteArticle(int id);
    }
}
