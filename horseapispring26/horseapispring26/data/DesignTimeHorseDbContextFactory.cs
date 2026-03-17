using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace horseapispring26.Data;

public class DesignTimeHorseDbContextFactory : IDesignTimeDbContextFactory<HorseDbContext>
{
    public HorseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HorseDbContext>();

        // Use a design-time connection string; no actual connection is made during migration scaffolding
        var connectionString = Environment.GetEnvironmentVariable("HORSEAPI_NOAUTH_DESIGNTIME_CONNECTION")
                               ?? "Server=(localdb)\\mssqllocaldb;Database=HorseDbNoAuthDesignTime;Trusted_Connection=True;TrustServerCertificate=true";

        optionsBuilder.UseSqlServer(connectionString);
        return new HorseDbContext(optionsBuilder.Options);
    }
}
