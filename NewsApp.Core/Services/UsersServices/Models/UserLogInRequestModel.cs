namespace NewsApp.Core.Services.UsersServices.Models
{
    public class UserLogInRequestModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
