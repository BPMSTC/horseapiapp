using horseapispring26.Models;

namespace horseapispring26.Repositories
{
    public interface IHorseRepository
    {
        Task<PagedResult<Horse>> GetAllHorsesAsync(int pageNumber, int pageSize);
        Task<Horse?> GetHorseByIdAsync(int id);
        Task<Horse?> GetHorseByRegistrationNumberAsync(string registrationNumber);
        Task<Horse> CreateHorseAsync(Horse horse);
        Task<Horse?> UpdateHorseAsync(int id, Horse horse);
        Task<bool> DeleteHorseAsync(int id);
        Task<PagedResult<Horse>> SearchHorsesAsync(string searchTerm, int pageNumber, int pageSize);
        Task<PagedResult<Horse>> GetHorsesByOwnerAsync(string owner, int pageNumber, int pageSize);
        Task<PagedResult<Horse>> GetHorsesByTrainerAsync(string trainer, int pageNumber, int pageSize);
        Task<PagedResult<Horse>> GetTopEarnersAsync(int pageNumber, int pageSize);
        Task<PagedResult<Horse>> GetHorsesByGenderAsync(Gender gender, int pageNumber, int pageSize);
    }
}
