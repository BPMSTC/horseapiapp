using horseapispring26.Models;
using System.Text.Json;

namespace horseapispring26.Repositories
{
    public class JsonHorseRepository(IConfiguration configuration, ILogger<JsonHorseRepository> logger) : IHorseRepository
    {
        private readonly string _jsonFilePath = configuration["DataSources:JsonFile:Path"] ?? "data/horses.json";
        private List<Horse> _horses = new();
        private bool _isLoaded;

        private void EnsureHorsesLoaded()
        {
            if (_isLoaded)
            {
                return;
            }

            LoadHorsesFromFile();
            _isLoaded = true;
        }

        private void LoadHorsesFromFile()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    var jsonContent = File.ReadAllText(_jsonFilePath);
                    _horses = JsonSerializer.Deserialize<List<Horse>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Horse>();
                    logger.LogInformation($"Loaded {_horses.Count} horses from {_jsonFilePath}");
                }
                else
                {
                    logger.LogWarning($"JSON file not found at {_jsonFilePath}. Starting with empty list.");
                    _horses = new List<Horse>();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error loading horses from {_jsonFilePath}");
                _horses = new List<Horse>();
            }
        }

        private async Task SaveHorsesToFileAsync()
        {
            try
            {
                var directory = Path.GetDirectoryName(_jsonFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var jsonContent = JsonSerializer.Serialize(_horses, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(_jsonFilePath, jsonContent);
                logger.LogInformation($"Saved {_horses.Count} horses to {_jsonFilePath}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error saving horses to {_jsonFilePath}");
                throw;
            }
        }

        public async Task<PagedResult<Horse>> GetAllHorsesAsync(int pageNumber, int pageSize)
        {
            EnsureHorsesLoaded();
            return await Task.FromResult(Paginate(_horses.AsEnumerable(), pageNumber, pageSize));
        }

        public async Task<Horse?> GetHorseByIdAsync(int id)
        {
            EnsureHorsesLoaded();
            return await Task.FromResult(_horses.FirstOrDefault(h => h.Id == id));
        }

        public async Task<Horse?> GetHorseByRegistrationNumberAsync(string registrationNumber)
        {
            EnsureHorsesLoaded();
            return await Task.FromResult(_horses.FirstOrDefault(h => 
                h.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Horse> CreateHorseAsync(Horse horse)
        {
            EnsureHorsesLoaded();

            // Generate new ID
            horse.Id = _horses.Count > 0 ? _horses.Max(h => h.Id) + 1 : 1;

            // Check for duplicate registration number
            if (_horses.Any(h => h.RegistrationNumber.Equals(horse.RegistrationNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A horse with registration number '{horse.RegistrationNumber}' already exists.");
            }

            _horses.Add(horse);
            await SaveHorsesToFileAsync();
            logger.LogInformation($"Created horse with ID {horse.Id} and registration number {horse.RegistrationNumber}");
            return horse;
        }

        public async Task<Horse?> UpdateHorseAsync(int id, Horse horse)
        {
            EnsureHorsesLoaded();

            var existingHorse = _horses.FirstOrDefault(h => h.Id == id);
            if (existingHorse == null)
            {
                return null;
            }

            // Check for duplicate registration number (excluding current horse)
            if (_horses.Any(h => h.Id != id && h.RegistrationNumber.Equals(horse.RegistrationNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A horse with registration number '{horse.RegistrationNumber}' already exists.");
            }

            // Update properties
            existingHorse.Name = horse.Name;
            existingHorse.RegistrationNumber = horse.RegistrationNumber;
            existingHorse.DateOfBirth = horse.DateOfBirth;
            existingHorse.Gender = horse.Gender;
            existingHorse.Color = horse.Color;
            existingHorse.Sire = horse.Sire;
            existingHorse.Dam = horse.Dam;
            existingHorse.BreederName = horse.BreederName;
            existingHorse.PictureUrl = horse.PictureUrl;
            existingHorse.TotalRacesRun = horse.TotalRacesRun;
            existingHorse.Wins = horse.Wins;
            existingHorse.Places = horse.Places;
            existingHorse.Shows = horse.Shows;
            existingHorse.CareerEarnings = horse.CareerEarnings;
            existingHorse.CurrentOwner = horse.CurrentOwner;
            existingHorse.Trainer = horse.Trainer;

            await SaveHorsesToFileAsync();
            logger.LogInformation($"Updated horse with ID {id}");
            return existingHorse;
        }

        public async Task<bool> DeleteHorseAsync(int id)
        {
            EnsureHorsesLoaded();

            var horse = _horses.FirstOrDefault(h => h.Id == id);
            if (horse == null)
            {
                return false;
            }

            _horses.Remove(horse);
            await SaveHorsesToFileAsync();
            logger.LogInformation($"Deleted horse with ID {id}");
            return true;
        }

        public async Task<PagedResult<Horse>> SearchHorsesAsync(string searchTerm, int pageNumber, int pageSize)
        {
            EnsureHorsesLoaded();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllHorsesAsync(pageNumber, pageSize);
            }

            var term = searchTerm.ToLowerInvariant();
            var filtered = _horses.Where(h =>
                h.Name.ToLowerInvariant().Contains(term) ||
                h.RegistrationNumber.ToLowerInvariant().Contains(term) ||
                (h.Sire?.ToLowerInvariant().Contains(term) ?? false) ||
                (h.Dam?.ToLowerInvariant().Contains(term) ?? false) ||
                (h.CurrentOwner?.ToLowerInvariant().Contains(term) ?? false) ||
                (h.Trainer?.ToLowerInvariant().Contains(term) ?? false) ||
                h.Color.ToLowerInvariant().Contains(term)
            );

            return await Task.FromResult(Paginate(filtered, pageNumber, pageSize));
        }

        public async Task<PagedResult<Horse>> GetHorsesByOwnerAsync(string owner, int pageNumber, int pageSize)
        {
            EnsureHorsesLoaded();

            var filtered = _horses.Where(h =>
                !string.IsNullOrEmpty(h.CurrentOwner) &&
                h.CurrentOwner.Contains(owner, StringComparison.OrdinalIgnoreCase));

            return await Task.FromResult(Paginate(filtered, pageNumber, pageSize));
        }

        public async Task<PagedResult<Horse>> GetHorsesByTrainerAsync(string trainer, int pageNumber, int pageSize)
        {
            EnsureHorsesLoaded();

            var filtered = _horses.Where(h =>
                !string.IsNullOrEmpty(h.Trainer) &&
                h.Trainer.Contains(trainer, StringComparison.OrdinalIgnoreCase));

            return await Task.FromResult(Paginate(filtered, pageNumber, pageSize));
        }

        public async Task<PagedResult<Horse>> GetTopEarnersAsync(int pageNumber, int pageSize)
        {
            EnsureHorsesLoaded();

            var ordered = _horses.OrderByDescending(h => h.CareerEarnings);
            return await Task.FromResult(Paginate(ordered, pageNumber, pageSize));
        }

        public async Task<PagedResult<Horse>> GetHorsesByGenderAsync(Gender gender, int pageNumber, int pageSize)
        {
            EnsureHorsesLoaded();

            var filtered = _horses.Where(h => h.Gender == gender);
            return await Task.FromResult(Paginate(filtered, pageNumber, pageSize));
        }

        private static PagedResult<Horse> Paginate(IEnumerable<Horse> source, int pageNumber, int pageSize)
        {
            var sourceList = source.ToList();
            var items = sourceList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Horse>
            {
                Items = items,
                TotalCount = sourceList.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
