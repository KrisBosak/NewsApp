﻿namespace NewsApp.Core.Services.UsersServices.Models
{
    public class UserSignUpRequestModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAuthor { get; set; }
    }
}
