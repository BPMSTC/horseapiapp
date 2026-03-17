using horseapispring26.Data;
using horseapispring26.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace horseapispring26.Repositories;

public class SqlHorseRepository : IHorseRepository
{
    private readonly IHorseDataContext _db;
    private readonly ILogger<SqlHorseRepository> logger;

    public SqlHorseRepository(IHorseDataContext db, ILogger<SqlHorseRepository> logger)
    {
        _db = db;
        this.logger = logger;
    }

    public async Task<PagedResult<Horse>> GetAllHorsesAsync(int pageNumber, int pageSize)
    {
        return await PaginateAsync(_db.Horses.AsNoTracking(), pageNumber, pageSize);
    }

    public async Task<Horse?> GetHorseByIdAsync(int id)
    {
        return await _db.Horses.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Horse?> GetHorseByRegistrationNumberAsync(string registrationNumber)
    {
        // Assuming CI collation, exact match is case-insensitive in SQL Server
        return await _db.Horses.AsNoTracking()
            .FirstOrDefaultAsync(h => h.RegistrationNumber == registrationNumber);
    }

    public async Task<Horse> CreateHorseAsync(Horse horse)
    {
        // Pre-check for duplicate registration number to keep behavior parity
        var exists = await _db.Horses.AsNoTracking()
            .AnyAsync(h => h.RegistrationNumber == horse.RegistrationNumber);
        if (exists)
        {
            throw new InvalidOperationException($"A horse with registration number '{horse.RegistrationNumber}' already exists.");
        }

        // Ensure soft delete flag is false on create
        horse.IsDeleted = false;

        _db.Horses.Add(horse);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new InvalidOperationException($"A horse with registration number '{horse.RegistrationNumber}' already exists.", ex);
        }

        logger.LogInformation("Created horse with ID {Id} and registration number {Reg}", horse.Id, horse.RegistrationNumber);
        return horse;
    }

    public async Task<Horse?> UpdateHorseAsync(int id, Horse horse)
    {
        var existing = await _db.Horses.FirstOrDefaultAsync(h => h.Id == id);
        if (existing == null)
        {
            return null;
        }

        // Check for duplicate registration number on other horses
        var duplicate = await _db.Horses.AsNoTracking()
            .AnyAsync(h => h.Id != id && h.RegistrationNumber == horse.RegistrationNumber);
        if (duplicate)
        {
            throw new InvalidOperationException($"A horse with registration number '{horse.RegistrationNumber}' already exists.");
        }

        existing.Name = horse.Name;
        existing.RegistrationNumber = horse.RegistrationNumber;
        existing.DateOfBirth = horse.DateOfBirth;
        existing.Gender = horse.Gender;
        existing.Color = horse.Color;
        existing.Sire = horse.Sire;
        existing.Dam = horse.Dam;
        existing.BreederName = horse.BreederName;
        existing.PictureUrl = horse.PictureUrl;
        existing.TotalRacesRun = horse.TotalRacesRun;
        existing.Wins = horse.Wins;
        existing.Places = horse.Places;
        existing.Shows = horse.Shows;
        existing.CareerEarnings = horse.CareerEarnings;
        existing.CurrentOwner = horse.CurrentOwner;
        existing.Trainer = horse.Trainer;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new InvalidOperationException($"A horse with registration number '{horse.RegistrationNumber}' already exists.", ex);
        }

        logger.LogInformation("Updated horse with ID {Id}", id);
        return existing;
    }

    public async Task<bool> DeleteHorseAsync(int id)
    {
        var horse = await _db.Horses.FirstOrDefaultAsync(h => h.Id == id);
        if (horse == null)
        {
            return false;
        }

        horse.IsDeleted = true;
        await _db.SaveChangesAsync();
        logger.LogInformation("Soft-deleted horse with ID {Id}", id);
        return true;
    }

    public async Task<PagedResult<Horse>> SearchHorsesAsync(string searchTerm, int pageNumber, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllHorsesAsync(pageNumber, pageSize);
        }

        // Case-insensitive by default under CI collation
        var query = _db.Horses.AsNoTracking().Where(h =>
            h.Name.Contains(searchTerm) ||
            h.RegistrationNumber.Contains(searchTerm) ||
            (h.Sire != null && h.Sire.Contains(searchTerm)) ||
            (h.Dam != null && h.Dam.Contains(searchTerm)) ||
            (h.CurrentOwner != null && h.CurrentOwner.Contains(searchTerm)) ||
            (h.Trainer != null && h.Trainer.Contains(searchTerm)) ||
            h.Color.Contains(searchTerm)
        );

        return await PaginateAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedResult<Horse>> GetHorsesByOwnerAsync(string owner, int pageNumber, int pageSize)
    {
        var query = _db.Horses.AsNoTracking()
            .Where(h => h.CurrentOwner != null && h.CurrentOwner.Contains(owner));

        return await PaginateAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedResult<Horse>> GetHorsesByTrainerAsync(string trainer, int pageNumber, int pageSize)
    {
        var query = _db.Horses.AsNoTracking()
            .Where(h => h.Trainer != null && h.Trainer.Contains(trainer));

        return await PaginateAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedResult<Horse>> GetTopEarnersAsync(int pageNumber, int pageSize)
    {
        var query = _db.Horses.AsNoTracking()
            .OrderByDescending(h => h.CareerEarnings);

        return await PaginateAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedResult<Horse>> GetHorsesByGenderAsync(Gender gender, int pageNumber, int pageSize)
    {
        var query = _db.Horses.AsNoTracking()
            .Where(h => h.Gender == gender);

        return await PaginateAsync(query, pageNumber, pageSize);
    }

    private static async Task<PagedResult<Horse>> PaginateAsync(IQueryable<Horse> query, int pageNumber, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Horse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx)
        {
            // 2601: Cannot insert duplicate key row in object with unique index.
            // 2627: Violation of UNIQUE KEY constraint.
            return sqlEx.Number == 2601 || sqlEx.Number == 2627;
        }
        return false;
    }
}
