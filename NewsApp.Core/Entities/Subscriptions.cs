namespace NewsApp.Core.Entities
{
    public class Subscriptions
    {
        public int Id { get; set; }
        public DateTimeOffset SubscribedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual Users User { get; set; } = new();
        public int? CategoryId { get; set; } // Optional if subscribing to authors
        public virtual Categories Category { get; set; } = new();
        public string? AuthorId { get; set; }   // Optional if subscribing to categories
        public virtual Users Author { get; set; } = new();
    }
}
