using horseapispring26.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace horseapispring26.Data;

public interface IHorseDataContext
{
    DbSet<Horse> Horses { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
