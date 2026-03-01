using Microsoft.Extensions.Logging;

namespace AdventureRental.Infrastructure.Services;

public class SeedDataService
{
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(ILogger<SeedDataService> logger)
    {
        _logger = logger;
    }

    public Task SeedAsync()
    {
        _logger.LogInformation(
            "User roles and admin accounts are managed in Entra External ID. No local seeding required.");
        return Task.CompletedTask;
    }
}
