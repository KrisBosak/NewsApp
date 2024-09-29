using Microsoft.AspNetCore.Identity;

namespace NewsApp.Core.Entities
{        
    // We could potentially add Articles and Media here too and then have a profile for users but this is out of scope at the moment.
    public class User : IdentityUser
    {
        public bool IsAuthor { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
        public virtual ICollection<Subscription> UserSubscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<Subscription> AuthorsSubscribers { get; set; } = new List<Subscription>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
