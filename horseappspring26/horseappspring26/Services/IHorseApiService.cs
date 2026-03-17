using horseappspring26.Models;

namespace horseappspring26.Services;

public interface IHorseApiService
{
    Task<PagedResponse<Horse>> GetHorsesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResponse<Horse>> SearchHorsesAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Horse?> GetHorseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Horse> CreateHorseAsync(HorseUpsertRequest request, CancellationToken cancellationToken = default);
    Task<Horse> UpdateHorseAsync(int id, HorseUpsertRequest request, CancellationToken cancellationToken = default);
    Task DeleteHorseAsync(int id, CancellationToken cancellationToken = default);
}
