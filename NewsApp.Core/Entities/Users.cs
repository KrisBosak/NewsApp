using Microsoft.AspNetCore.Identity;

namespace NewsApp.Core.Entities
{        
    // We could potentially add Articles and Media here too and then have a profile for users but this is out of scope at the moment.
    public class Users : IdentityUser
    {
        public bool IsAuthor { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<Articles> Articles { get; set; } = new List<Articles>();
        public virtual ICollection<Subscriptions> UserSubscriptions { get; set; } = new List<Subscriptions>();
        public virtual ICollection<Subscriptions> AuthorsSubscribers { get; set; } = new List<Subscriptions>();
        public virtual ICollection<Comments> Comments { get; set; } = new List<Comments>();
        public virtual ICollection<Likes> Likes { get; set; } = new List<Likes>();
    }
}
