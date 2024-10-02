using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsApp.Core.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Services.UsersServices.Interfaces;
using NewsApp.Core.Services.UsersServices.Models;
using NewsApp.Core.Services.Utils;

namespace NewsApp.Core.Services.UsersServices
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly NewsDbContext _context;

        public UsersService(UserManager<User> userManager, NewsDbContext context)
        {
            this._userManager = userManager;
            _context = context;
        }
        public async Task<PagedResult<User>> GetAllUsers(QueryParameters queryHelpers)
        {
            var query = _userManager.Users
                .Include(x => x.Articles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryHelpers.FilterValue))
            {
                switch (queryHelpers.FilterBy.ToLower().Replace(" ", ""))
                {
                    case "username":
                        query = query.Where(x => x.UserName != null && x.UserName.Contains(queryHelpers.FilterValue));
                        break;
                    default:
                        query = query.Where(x => x.UserName != null && x.UserName.Contains(queryHelpers.FilterValue)); // I wouldn't usualy just duplicated code but this is for show only.
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(queryHelpers.OrderBy))
            {
                switch (queryHelpers.OrderBy.ToLower().Replace(" ", ""))
                {
                    case "username":
                        query = queryHelpers.OrderByDescending ? query.OrderByDescending(x => x.UserName) : query.OrderBy(x => x.UserName);
                        break;
                    default:
                        query = queryHelpers.OrderByDescending ? query.OrderByDescending(x => x.UserName) : query.OrderBy(x => x.UserName); // I wouldn't usualy just duplicated code but this is for show only.
                        break;
                }
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryHelpers.PageNumber - 1) * queryHelpers.PageSize).Take(queryHelpers.PageSize).ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = queryHelpers.PageNumber,
                PageSize = queryHelpers.PageSize,
                TotalPages = (int)Math.Ceiling((double) totalItems / queryHelpers.PageSize)
            };
        }

        public async Task<PagedResult<User>> GetAuthors(QueryParameters queryHelpers)
        {
            var query = _userManager.Users
                .Where(x => x.IsAuthor)
                .Include(x => x.Articles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryHelpers.FilterValue))
            {
                switch (queryHelpers.FilterBy.ToLower().Replace(" ", ""))
                {
                    case "username":
                        query = query.Where(x => x.UserName != null && x.UserName.Contains(queryHelpers.FilterValue));
                        break;
                    default:
                        query = query.Where(x => x.UserName != null && x.UserName.Contains(queryHelpers.FilterValue)); // I wouldn't usualy just duplicated code but this is for show only.
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(queryHelpers.OrderBy))
            {
                switch (queryHelpers.OrderBy.ToLower().Replace(" ", ""))
                {
                    case "username":
                        query = queryHelpers.OrderByDescending ? query.OrderByDescending(x => x.UserName) : query.OrderBy(x => x.UserName);
                        break;
                    default:
                        query = queryHelpers.OrderByDescending ? query.OrderByDescending(x => x.UserName) : query.OrderBy(x => x.UserName); // I wouldn't usualy just duplicated code but this is for show only.
                        break;
                }
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip((queryHelpers.PageNumber - 1) * queryHelpers.PageSize).Take(queryHelpers.PageSize).ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = queryHelpers.PageNumber,
                PageSize = queryHelpers.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / queryHelpers.PageSize)
            };
        }

        public async Task<User?> GetUser(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IdentityResult?> UpdateUser(UserUpdateRequestModel request)
        {
            IdentityResult result = new();
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (userToUpdate == null) return null;
                
            userToUpdate.UserName = request.UserName;
            userToUpdate.Email = request.Email;
            userToUpdate.EmailConfirmed = true;
            userToUpdate.IsAuthor = request.IsAuthor;
            userToUpdate.IsActive = true;

            if (request.NewPassword != request.OldPassword)
            {
                var passChangeResult = await _userManager.ChangePasswordAsync(userToUpdate, request.OldPassword, request.NewPassword);
                if(!passChangeResult.Succeeded) return passChangeResult;
            }

            return await _userManager.UpdateAsync(userToUpdate);
        }

        public async Task<bool> DeactivateOrDeleteUser(string id, bool permanentlyDelete)
        {
            var userToDeleteOrDeactivate = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (userToDeleteOrDeactivate == null) return false;

            if (permanentlyDelete)
            {
                _context.Remove(userToDeleteOrDeactivate);
                await _context.SaveChangesAsync();

                return true;
            }

            userToDeleteOrDeactivate.IsActive = false;
            await _userManager.UpdateAsync(userToDeleteOrDeactivate);

            return true;
        }
    }
}
