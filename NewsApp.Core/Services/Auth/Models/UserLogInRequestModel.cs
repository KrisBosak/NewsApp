namespace NewsApp.Core.Services.Auth.Models
{
    public class UserLogInRequestModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
