using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NewsApp.Core.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Services.ArticlesServices.Interfaces;
using NewsApp.Core.Services.ArticlesServices.Models;
using NewsApp.Core.Services.Utils;

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
            return await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<Article>> GetArticles(QueryParameters queryHelpers)
        {
            var query = _context.Articles
                .Where(x => x.IsPublished)
                .Include(x => x.Author)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(queryHelpers.FilterValue))
            {
                switch (queryHelpers.FilterBy.ToLower().Replace(" ", ""))
                {
                    case "title":
                        query = query.Where(x => x.Title != null && x.Title.Contains(queryHelpers.FilterValue));
                        break;
                    default:
                        query = query.Where(x => x.Title != null && x.Title.Contains(queryHelpers.FilterValue)); // I wouldn't usualy just duplicated code but this is for show only.
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(queryHelpers.OrderBy))
            {
                switch (queryHelpers.OrderBy.ToLower().Replace(" ", ""))
                {
                    case "publisheddate":
                        query = queryHelpers.OrderByDescending ? query.OrderByDescending(x => x.PublishedDate) : query.OrderBy(x => x.PublishedDate);
                        break;
                    default:
                        query = queryHelpers.OrderByDescending ? query.OrderByDescending(x => x.PublishedDate) : query.OrderBy(x => x.PublishedDate); // I wouldn't usualy just duplicated code but this is for show only.
                        break;
                }
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryHelpers.PageNumber - 1) * queryHelpers.PageSize).Take(queryHelpers.PageSize).ToListAsync();

            return new PagedResult<Article>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = queryHelpers.PageNumber,
                PageSize = queryHelpers.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / queryHelpers.PageSize)
            };

        }

        public async Task<Article?> CreateArticle(ArticleCreateRequestModel requestModel, User user)
        {
            if (requestModel.Media == null) throw new Exception("Please provide a media file (a picture for example).");
            Media mediaDetails = await GetMedia(requestModel.Media);

            if (!_context.Categories.Any(x => x.Id == requestModel.CategoryId)) throw new Exception("Provided category does not exist. Please check and try again.");

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

            mediaDetails.Article = newArticle;

            _context.Articles.Add(newArticle);
            _context.Media.Add(mediaDetails);
            
            await _context.SaveChangesAsync();

            return newArticle;
        }

        public async Task<Article?> UpdateArticle(ArticleUpdateRequestModel requestModel)
        {
            if (requestModel.Media == null) throw new Exception("Please provide a media file (a picture for example).");
            Media mediaDetails = await GetMedia(requestModel.Media);

            if (!_context.Categories.Any(x => x.Id == requestModel.CategoryId)) throw new Exception("Provided category does not exist. Please check and try again.");

            Article? articleToUpdate = await _context.Articles.FirstOrDefaultAsync(x => x.Id == requestModel.Id);
            Media? mediaToUpdate = await _context.Media.FirstOrDefaultAsync(x => x.ArticleId == requestModel.Id);

            if (articleToUpdate == null) return null;

            articleToUpdate.Title = requestModel.Title;
            articleToUpdate.Content = requestModel.Content;
            articleToUpdate.Summary = requestModel.Summary;
            articleToUpdate.IsPublished = requestModel.IsPublished;
            articleToUpdate.UpdatedAt = DateTimeOffset.UtcNow;
            articleToUpdate.CategoryId = requestModel.CategoryId;

            if (mediaToUpdate != null)
            {
                if (File.Exists(mediaToUpdate.Path))
                {
                    File.Delete(mediaToUpdate.Path);
                }

                mediaToUpdate.Article = articleToUpdate;
                mediaToUpdate.Path = mediaDetails.Path;
                mediaToUpdate.Type = mediaDetails.Type;
                mediaToUpdate.Name = mediaDetails.Name;

                _context.Media.Update(mediaToUpdate);
            }
            else
            {
                mediaDetails.Article = articleToUpdate;

                _context.Media.Add(mediaDetails);
            }

            _context.Articles.Update(articleToUpdate);
            await _context.SaveChangesAsync();

            return articleToUpdate;
        }

        public async Task<bool> DeleteArticle(int id)
        {
            Article? articleForDelete = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);

            if (articleForDelete == null) return false;

            _context.Articles.Remove(articleForDelete);

            await _context.SaveChangesAsync();

            return true;
        }

        private static async Task<Media> GetMedia(IFormFile file)
        {
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

            return new Media
            {
                Name = fileName,
                Path = filePath,
                Type = file.ContentType,
            };
        }
    }
}
