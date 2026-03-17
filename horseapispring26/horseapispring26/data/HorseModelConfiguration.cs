using horseapispring26.Models;
using Microsoft.EntityFrameworkCore;

namespace horseapispring26.Data;

public static class HorseModelConfiguration
{
    public static void ConfigureHorseModel(ModelBuilder modelBuilder)
    {
        var horse = modelBuilder.Entity<Horse>();
        horse.ToTable("Horses");

        horse.HasKey(h => h.Id);
        horse.Property(h => h.Id)
             .ValueGeneratedOnAdd();

        horse.Property(h => h.Name)
             .IsRequired()
             .HasMaxLength(200);

        horse.Property(h => h.RegistrationNumber)
             .IsRequired()
             .HasMaxLength(50);

        horse.HasIndex(h => h.RegistrationNumber)
             .IsUnique();

        horse.Property(h => h.DateOfBirth)
             .IsRequired()
             .HasColumnType("date");

        horse.Property(h => h.Gender)
             .IsRequired()
             .HasConversion<short>()
             .HasColumnType("smallint");

        horse.Property(h => h.Color)
             .IsRequired()
             .HasMaxLength(50);

        horse.Property(h => h.Sire)
             .HasMaxLength(200);

        horse.Property(h => h.Dam)
             .HasMaxLength(200);

        horse.Property(h => h.BreederName)
             .HasMaxLength(200);

        horse.Property(h => h.PictureUrl)
             .HasMaxLength(512);

        horse.Property(h => h.TotalRacesRun).HasDefaultValue(0);
        horse.Property(h => h.Wins).HasDefaultValue(0);
        horse.Property(h => h.Places).HasDefaultValue(0);
        horse.Property(h => h.Shows).HasDefaultValue(0);
        horse.Property(h => h.CareerEarnings).HasColumnType("decimal(18,2)").HasDefaultValue(0);

        horse.Property(h => h.CurrentOwner).HasMaxLength(200);
        horse.Property(h => h.Trainer).HasMaxLength(200);

        horse.Property(h => h.IsDeleted)
             .HasDefaultValue(false)
             .IsRequired();

        horse.HasQueryFilter(h => !h.IsDeleted);

        horse.HasIndex(h => h.Gender);
        horse.HasIndex(h => h.CareerEarnings);
    }
}
