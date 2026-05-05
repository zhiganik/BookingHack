using BookingHack.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookingHack.Infrastructure.PostgreSql.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RoleManager<IdentityRole>>>();

        foreach (var role in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;

            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
                logger.LogInformation("Seeded role: {Role}", role);
            else
                logger.LogError("Failed to seed role {Role}: {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
