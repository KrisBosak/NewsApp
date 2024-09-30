using Microsoft.AspNetCore.Identity;

namespace NewsApp.Core.Entities
{        
    // We could potentially add Articles and Media here too and then have a profile for users but this is out of scope at the moment.
    public class User : IdentityUser
    {
        public bool IsAuthor { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<Article>? Articles { get; set; }
        public virtual ICollection<Subscription>? UserSubscriptions { get; set; }
        public virtual ICollection<Subscription>? AuthorsSubscribers { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }
    }
}
