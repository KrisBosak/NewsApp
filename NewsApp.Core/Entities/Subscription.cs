namespace NewsApp.Core.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public DateTimeOffset SubscribedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
        public int? CategoryId { get; set; } // Optional if subscribing to authors
        public virtual Category? Category { get; set; }
        public string? AuthorId { get; set; }   // Optional if subscribing to categories
        public virtual User? Author { get; set; }
    }
}
