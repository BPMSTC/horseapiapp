using horseapispring26.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace horseapispring26.Data;

public class AuthHorseDbContext : IdentityDbContext<IdentityUser>, IHorseDataContext
{
    public AuthHorseDbContext(DbContextOptions<AuthHorseDbContext> options) : base(options)
    {
    }

    public DbSet<Horse> Horses => Set<Horse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        HorseModelConfiguration.ConfigureHorseModel(modelBuilder);
    }
}
