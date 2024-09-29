namespace NewsApp.Core.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTimeOffset PublishedDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public virtual User Author { get; set; } = new();
        // I was thinking about making this a one to many relationship but... I prefer when categories are specific.
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = new();
        // Same as category really, I feel like one picture should be enough per news
        public int MediaId { get; set; }
        public virtual Media Media { get; set; } = new();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
