namespace NewsApp.Core.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
        public int? ArticleId { get; set; } // optional because you can like either the article or a comment
        public virtual Article? Article { get; set; }
        public int? CommentId { get; set; } // optional because you can like either the article or a comment
        public virtual Comment? Comment { get; set; }
    }
}
