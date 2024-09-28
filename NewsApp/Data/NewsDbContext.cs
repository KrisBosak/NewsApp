using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsApp.Core.Entities;

namespace NewsApp.Api.Data
{
    public class NewsDbContext : IdentityDbContext<Users>
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
