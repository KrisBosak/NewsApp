namespace NewsApp.Core.Entities
{
    public class Likes
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual Users User { get; set; } = new();
        public int? ArticleId { get; set; } // optional because you can like either the article or a comment
        public virtual Articles Article { get; set; } = new();
        public int? CommentId { get; set; } // optional because you can like either the article or a comment
        public virtual Comments Comment { get; set; } = new();
    }
}
