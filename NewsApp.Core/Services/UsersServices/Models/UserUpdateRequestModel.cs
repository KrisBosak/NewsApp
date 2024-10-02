namespace NewsApp.Core.Services.UsersServices.Models
{
    public class UserUpdateRequestModel
    {
        public string Id { get; set; } = string.Empty;  
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public bool IsAuthor { get; set; }
    }
}
