namespace horseapispring26.Models.DTOs;

public static class HorseMappings
{
    public static HorseResponseDto ToResponseDto(this Horse horse)
    {
        return new HorseResponseDto
        {
            Id = horse.Id,
            Name = horse.Name,
            RegistrationNumber = horse.RegistrationNumber,
            DateOfBirth = horse.DateOfBirth,
            Gender = horse.Gender,
            Color = horse.Color,
            Sire = horse.Sire,
            Dam = horse.Dam,
            BreederName = horse.BreederName,
            PictureUrl = horse.PictureUrl,
            TotalRacesRun = horse.TotalRacesRun,
            Wins = horse.Wins,
            Places = horse.Places,
            Shows = horse.Shows,
            CareerEarnings = horse.CareerEarnings,
            CurrentOwner = horse.CurrentOwner,
            Trainer = horse.Trainer,
            WinPercentage = horse.WinPercentage,
            PlacePercentage = horse.PlacePercentage,
            ShowPercentage = horse.ShowPercentage,
            Age = horse.GetAge(DateTime.UtcNow)
        };
    }

    public static Horse ToEntity(this CreateHorseRequestDto request)
    {
        return new Horse
        {
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Color = request.Color,
            Sire = request.Sire,
            Dam = request.Dam,
            BreederName = request.BreederName,
            PictureUrl = request.PictureUrl,
            TotalRacesRun = request.TotalRacesRun,
            Wins = request.Wins,
            Places = request.Places,
            Shows = request.Shows,
            CareerEarnings = request.CareerEarnings,
            CurrentOwner = request.CurrentOwner,
            Trainer = request.Trainer
        };
    }

    public static Horse ToEntity(this UpdateHorseRequestDto request, int id)
    {
        return new Horse
        {
            Id = id,
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Color = request.Color,
            Sire = request.Sire,
            Dam = request.Dam,
            BreederName = request.BreederName,
            PictureUrl = request.PictureUrl,
            TotalRacesRun = request.TotalRacesRun,
            Wins = request.Wins,
            Places = request.Places,
            Shows = request.Shows,
            CareerEarnings = request.CareerEarnings,
            CurrentOwner = request.CurrentOwner,
            Trainer = request.Trainer
        };
    }

    public static PagedResponseDto<HorseResponseDto> ToResponseDto(this PagedResult<Horse> pagedHorses)
    {
        return new PagedResponseDto<HorseResponseDto>
        {
            Items = pagedHorses.Items.Select(h => h.ToResponseDto()).ToList(),
            PageNumber = pagedHorses.PageNumber,
            PageSize = pagedHorses.PageSize,
            TotalCount = pagedHorses.TotalCount,
            TotalPages = (int)Math.Ceiling(pagedHorses.TotalCount / (double)pagedHorses.PageSize)
        };
    }
}
