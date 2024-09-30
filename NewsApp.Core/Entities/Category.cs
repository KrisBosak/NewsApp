﻿namespace NewsApp.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

        public virtual ICollection<Article>? Articles { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
    }
}
