using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace horseapispring26.Data;

public class DesignTimeAuthHorseDbContextFactory : IDesignTimeDbContextFactory<AuthHorseDbContext>
{
    public AuthHorseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthHorseDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("HORSEAPI_AUTH_DESIGNTIME_CONNECTION")
                               ?? "Server=(localdb)\\mssqllocaldb;Database=HorseDbAuthDesignTime;Trusted_Connection=True;TrustServerCertificate=true";

        optionsBuilder.UseSqlServer(connectionString);
        return new AuthHorseDbContext(optionsBuilder.Options);
    }
}
