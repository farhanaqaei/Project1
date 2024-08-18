using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Project1.Infrastructure.UserManagement.Entities;

namespace Project1.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static async Task SeedRolesAndAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed roles
        var roles = new List<ApplicationRole>
        {
            new ApplicationRole { Name = "Admin" },
            new ApplicationRole { Name = "User" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }

        // Seed admin user
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@12345");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
