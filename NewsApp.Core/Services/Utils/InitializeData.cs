using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NewsApp.Core.Data;
using NewsApp.Core.Entities;
using NewsApp.Core.Entities.Enums;

namespace NewsApp.Core.Services.Utils
{
    public static class InitializeData
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in Enum.GetValues(typeof(RolesEnum)).Cast<RolesEnum>().Select(r => r.ToString()))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string adminEmail = "admin@newsapp.com";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    Email = adminEmail,
                    UserName = "admin",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, "Admin#1234");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RolesEnum.Admin.ToString());
                }
            }
        }

        public static async Task SeedCategories(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<NewsDbContext>();

            if (!dbContext.Categories.Any(x => x.Name == "General"))
            {
                Category category = new Category
                {
                    Name = "General",
                    Description = "Category that fits all descriptions",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
