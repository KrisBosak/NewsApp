namespace NewsApp.Core.Entities
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

        public virtual ICollection<Articles> Articles { get; set; } = new List<Articles>();
        public virtual ICollection<Subscriptions> Subscriptions { get; set; } = new List<Subscriptions>();
    }
}
