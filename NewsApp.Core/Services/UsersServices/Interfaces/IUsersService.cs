using Microsoft.AspNetCore.Identity;
using NewsApp.Core.Entities;
using NewsApp.Core.Services.UsersServices.Models;
using NewsApp.Core.Services.Utils;

namespace NewsApp.Core.Services.UsersServices.Interfaces
{
    public interface IUsersService
    {
        public Task<PagedResult<User>> GetAllUsers(QueryParameters queryHelpers);
        public Task<PagedResult<User>> GetAuthors(QueryParameters queryHelpers);
        public Task<User?> GetUser(string id);
        public Task<IdentityResult?> UpdateUser(UserUpdateRequestModel request);
        public Task<bool> DeactivateOrDeleteUser(string id, bool permanentlyDelete);
    }
}
