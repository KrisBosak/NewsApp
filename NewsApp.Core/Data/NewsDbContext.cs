using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsApp.Core.Entities;

namespace NewsApp.Core.Data
{
    public class NewsDbContext : IdentityDbContext<User>
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Articles
            builder.Entity<Article>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Title).HasColumnName("Title").HasMaxLength(250);
                entity.Property(e => e.Content).HasColumnName("Content").HasColumnType("nvarchar(max)");
                entity.Property(e => e.Summary).HasColumnName("Summary").HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasColumnName("ImageUrl");
                entity.Property(e => e.IsPublished).HasColumnName("IsPublished");
                entity.Property(e => e.PublishedDate).HasColumnName("PublishedDate");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
                entity.Property(e => e.ViewCount).HasColumnName("ViewCount");
                entity.Property(e => e.LikesCount).HasColumnName("LikesCount");
                entity.Property(e => e.CommentsCount).HasColumnName("CommentsCount");
                entity.Property(e => e.AuthorId).HasColumnName("AuthorId");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryId");
                entity.Property(e => e.MediaId).HasColumnName("MediaId");

                entity.HasOne(e => e.Author)
                    .WithMany(m => m.Articles)
                    .HasForeignKey(f => f.AuthorId)
                    .HasConstraintName("fk_Articles_Authors")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(m => m.Articles)
                    .HasForeignKey(f => f.CategoryId)
                    .HasConstraintName("fk_Articles_Categories")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Media)
                    .WithOne(m => m.Article)
                    .HasForeignKey<Article>(d => d.MediaId)
                    .HasConstraintName("fk_Articles_Media")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Users
            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.IsAuthor).HasColumnName("IsAuthor");
                entity.Property(e => e.IsActive).HasColumnName("IsActive");

                entity.HasMany(e => e.Articles)
                    .WithOne(e => e.Author)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Comments)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Likes)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserSubscriptions)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AuthorsSubscribers)
                    .WithOne(e => e.Author)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Categories
            builder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(125);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

                entity.HasMany(e => e.Articles)
                    .WithOne(e => e.Category)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Subscriptions)
                    .WithOne(e => e.Category)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Media
            builder.Entity<Media>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(250);
                entity.Property(e => e.Path).HasColumnName("Path").HasMaxLength(500);
                entity.Property(e => e.Type).HasColumnName("Type").HasMaxLength(50);

                entity.HasOne(e => e.Article)
                    .WithOne(a => a.Media)
                    .HasForeignKey<Media>(m => m.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Comments
            builder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Content).HasColumnName("Content").HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                entity.HasOne(e => e.Article)
                    .WithMany(a => a.Comments)
                    .HasForeignKey(e => e.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Likes
            builder.Entity<Like>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Article)
                    .WithMany(a => a.Likes)
                    .HasForeignKey(e => e.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Comment)
                    .WithMany(c => c.Likes)
                    .HasForeignKey(e => e.CommentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Subscriptions
            builder.Entity<Subscription>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.SubscribedAt).HasColumnName("SubscribedAt");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserSubscriptions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Subscriptions)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Author)
                    .WithMany(a => a.AuthorsSubscribers)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(builder);
        }
    }
}
