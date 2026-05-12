using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data;

/// <summary>
/// Seeds roles and an optional admin account.
/// </summary>
public static class IdentitySeeder
{
    private const string AdminRole = "Admin";
    private const string UserRole = "User";

    /// <summary>
    /// Seeds roles and optional admin user based on configuration.
    /// </summary>
    /// <param name="services">Service provider scope.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="logger">Logger instance.</param>
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await EnsureRoleAsync(roleManager, UserRole, logger);
        await EnsureRoleAsync(roleManager, AdminRole, logger);

        var adminEmail = configuration["SeedAdmin:Email"];
        var adminPassword = configuration["SeedAdmin:Password"];
        var adminUserName = configuration["SeedAdmin:UserName"] ?? adminEmail;

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogInformation("Admin seed skipped: missing SeedAdmin:Email or SeedAdmin:Password.");
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                logger.LogError("Admin user creation failed: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(adminUser, AdminRole);
            if (!addRoleResult.Succeeded)
            {
                logger.LogError("Assigning admin role failed: {Errors}",
                    string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task EnsureRoleAsync(
        RoleManager<IdentityRole> roleManager,
        string roleName,
        ILogger logger)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            logger.LogError("Role creation failed for {Role}: {Errors}",
                roleName,
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
