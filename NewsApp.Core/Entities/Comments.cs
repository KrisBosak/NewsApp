namespace NewsApp.Core.Entities
{
    public class Comments
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public int ArticleId { get; set; }
        public virtual Articles Article { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
        public virtual Users User { get; set; } = new();

        public virtual ICollection<Likes> Likes { get; set; } = new List<Likes>();
    }
}
