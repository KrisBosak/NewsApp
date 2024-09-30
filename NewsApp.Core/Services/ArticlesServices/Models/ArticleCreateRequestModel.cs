using Microsoft.AspNetCore.Http;
using NewsApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApp.Core.Services.ArticlesServices.Models
{
    public class ArticleCreateRequestModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public IFormFile Media { get; set; }
        public int CategoryId { get; set; }
    }
}
