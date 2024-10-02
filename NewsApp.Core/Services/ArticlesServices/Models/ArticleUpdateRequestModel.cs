using Microsoft.AspNetCore.Http;

namespace NewsApp.Core.Services.ArticlesServices.Models
{
    public class ArticleUpdateRequestModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public IFormFile? Media { get; set; }
        public int CategoryId { get; set; }
    }
}
