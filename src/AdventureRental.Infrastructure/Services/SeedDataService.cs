using AdventureRental.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AdventureRental.Infrastructure.Services;

public class SeedDataService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<SeedDataService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await EnsureRoleAsync("Admin");
        await EnsureRoleAsync("Customer");
        await EnsureAdminUserAsync();
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                _logger.LogInformation("Created role: {Role}", roleName);
            else
                _logger.LogError("Failed to create role {Role}: {Errors}", roleName,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private async Task EnsureAdminUserAsync()
    {
        var adminEmail = _configuration["SeedData:AdminEmail"] ?? "admin@adventurerental.com";
        var adminPassword = _configuration["SeedData:AdminPassword"] ?? "Admin@123456";

        if (await _userManager.FindByEmailAsync(adminEmail) != null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, "Admin");
            _logger.LogInformation("Created default admin user: {Email}", adminEmail);
        }
        else
        {
            _logger.LogError("Failed to create admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
