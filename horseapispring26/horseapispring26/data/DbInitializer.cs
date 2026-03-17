using System.Text.Json;
using horseapispring26.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace horseapispring26.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration, bool seedIdentity)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IHorseDataContext>();
        var logFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var log = logFactory.CreateLogger("DbInitializer");

        // Migrations are the single source of truth for schema creation and updates.
        // Retry to handle startup race conditions when the DB container is still becoming ready.
        var maxAttempts = configuration.GetValue<int?>("Database:MigrationRetryCount") ?? 6;
        var delayMs = configuration.GetValue<int?>("Database:MigrationRetryDelayMs") ?? 2000;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await db.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                log.LogWarning(ex,
                    "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelayMs}ms.",
                    attempt, maxAttempts, delayMs);
                await Task.Delay(delayMs);
            }
        }

        if (seedIdentity)
        {
            await SeedIdentityDataAsync(scope.ServiceProvider, log);
        }

        // Import from JSON if configured and table empty
        var importFlag = configuration.GetValue<bool>("DataSources:ImportFromJsonOnStart");
        if (!importFlag)
        {
            log.LogInformation("ImportFromJsonOnStart is disabled; skipping JSON import.");
            return;
        }

        var hasAny = await db.Horses.AsNoTracking().AnyAsync();

        if (hasAny)
        {
            log.LogInformation("Horses table already has data; skipping JSON import.");
            return;
        }

        var jsonPath = configuration["DataSources:JsonFile:Path"] ?? "data/horses.json";
        if (!Path.IsPathRooted(jsonPath))
        {
            var basePath = AppContext.BaseDirectory;
            jsonPath = Path.Combine(basePath, jsonPath);
        }

        if (!File.Exists(jsonPath))
        {
            log.LogWarning("JSON file not found at {Path}; skipping import.", jsonPath);
            return;
        }

        List<Horse>? horses;
        try
        {
            var json = await File.ReadAllTextAsync(jsonPath);
            horses = JsonSerializer.Deserialize<List<Horse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to read or deserialize JSON from {Path}", jsonPath);
            return;
        }

        if (horses == null || horses.Count == 0)
        {
            log.LogInformation("No horses to import from JSON.");
            return;
        }

        foreach (var h in horses)
        {
            h.IsDeleted = false;
        }

        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Horses] ON;");
            db.Horses.AddRange(horses);
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Horses] OFF;");
            await tx.CommitAsync();
            log.LogInformation("Imported {Count} horses from JSON.", horses.Count);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            log.LogError(ex, "Failed to import horses from JSON.");
        }
    }

    private static async Task SeedIdentityDataAsync(IServiceProvider services, ILogger log)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        const string adminRole = "Admin";
        const string adminEmail = "admin@horseapi.com";
        const string adminPassword = "Admin123!";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
            if (!roleResult.Succeeded)
            {
                log.LogWarning("Could not create role {Role}: {Errors}", adminRole, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!userResult.Succeeded)
            {
                log.LogWarning("Could not create seed admin user {Email}: {Errors}", adminEmail, string.Join(", ", userResult.Errors.Select(e => e.Description)));
                return;
            }

            log.LogInformation("Seeded Identity admin user {Email} with password {Password}", adminEmail, adminPassword);
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
            if (!addToRoleResult.Succeeded)
            {
                log.LogWarning("Could not assign {Role} to {Email}: {Errors}", adminRole, adminEmail, string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
