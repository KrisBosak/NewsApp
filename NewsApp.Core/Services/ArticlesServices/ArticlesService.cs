using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NewsApp.Core.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Services.ArticlesServices.Interfaces;
using NewsApp.Core.Services.ArticlesServices.Models;
using System.Text.Json.Serialization;

namespace NewsApp.Core.Services.ArticlesServices
{
    public class ArticlesService : IArticlesService
    {
        private readonly NewsDbContext _context;

        public ArticlesService(NewsDbContext context)
        {
            _context = context;
        }

        public async Task<Article?> GetArticle(int id)
        {
            return await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Article>> GetArticles()
        {
            return await _context.Articles
                .Where(x => x.IsPublished)
                .OrderByDescending(x => x.PublishedDate)
                .ToListAsync();
        }

        public async Task<int> CreateArticle(ArticleCreateRequestModel requestModel, User user)
        {
            MediaDetails mediaDetails = await GetMedia(requestModel.Media);

            if(!_context.Categories.Any(x => x.Id == requestModel.CategoryId)) return 0; // this should be changed - maybe validate in the controller but for sure make the return message much better

            var newArticle = new Article
            {
                Title = requestModel.Title,
                Content = requestModel.Content,
                Summary = requestModel.Summary,
                ImageUrl = mediaDetails.Path,
                IsPublished = requestModel.IsPublished,
                PublishedDate = requestModel.IsPublished ? DateTimeOffset.UtcNow : null,
                CreatedAt = DateTimeOffset.UtcNow,
                AuthorId = user.Id,
                CategoryId = requestModel.CategoryId
            };

            Media attachedMedia = new Media
            {
                Name = mediaDetails.Name,
                Path = mediaDetails.Path,
                Type = mediaDetails.Type,
                Article = newArticle
            };

            _context.Articles.Add(newArticle);
            _context.Media.Add(attachedMedia);
            
            await _context.SaveChangesAsync();

            return newArticle.Id;
        }

        public async Task<bool> DeleteArticle(int id)
        {
            Article? articleForDelete = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);

            if (articleForDelete == null) return false;

            _context.Articles.Remove(articleForDelete);

            await _context.SaveChangesAsync();

            return true;
        }

        private static async Task<MediaDetails> GetMedia(IFormFile? file)
        {
            if (file == null) return new MediaDetails();

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Media");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var filePath = Path.Combine(uploadDirectory, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return new MediaDetails
            {
                Name = fileName,
                Path = filePath,
                Type = file.ContentType,
            };
        }
    }

    struct MediaDetails
    {
        public string Name;
        [JsonIgnore]
        public string Path;
        [JsonIgnore]
        public string Type;
    }
}
