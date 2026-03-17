using horseapispring26.Models;
using Microsoft.EntityFrameworkCore;

namespace horseapispring26.Data;

public class HorseDbContext : DbContext, IHorseDataContext
{
    public HorseDbContext(DbContextOptions<HorseDbContext> options) : base(options)
    {
    }

    public DbSet<Horse> Horses => Set<Horse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        HorseModelConfiguration.ConfigureHorseModel(modelBuilder);
    }
}
