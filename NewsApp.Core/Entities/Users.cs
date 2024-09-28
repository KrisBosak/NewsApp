using Microsoft.AspNetCore.Identity;

namespace NewsApp.Core.Entities
{
    public class Users : IdentityUser
    {
        public bool IsAuthor { get; set; }
    }
}
