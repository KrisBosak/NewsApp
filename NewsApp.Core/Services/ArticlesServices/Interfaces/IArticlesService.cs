using NewsApp.Core.Entities;
using NewsApp.Core.Services.ArticlesServices.Models;
using NewsApp.Core.Services.Utils;

namespace NewsApp.Core.Services.ArticlesServices.Interfaces
{
    public interface IArticlesService
    {
        public Task<Article?> GetArticle(int id);
        public Task<PagedResult<Article>> GetArticles(QueryParameters queryHelpers); 
        public Task<Article?> CreateArticle(ArticleCreateRequestModel requestModel, User user);
        public Task<Article?> UpdateArticle(ArticleUpdateRequestModel requestModel);
        public Task<bool> DeleteArticle(int id);
    }
}
