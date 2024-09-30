namespace NewsApp.Core.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public int ArticleId { get; set; }
        public virtual Article? Article { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }

        public virtual ICollection<Like>? Likes { get; set; }
    }
}
