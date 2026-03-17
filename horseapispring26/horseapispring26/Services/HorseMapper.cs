using horseapispring26.Models;
using horseapispring26.Models.DTOs;

namespace horseapispring26.Services;

public class HorseMapper(TimeProvider timeProvider) : IHorseMapper
{
    public HorseResponseDto ToHorseDto(Horse horse)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

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
            Age = horse.GetAge(now)
        };
    }

    public PagedResponseDto<HorseResponseDto> ToHorseDtoPage(PagedResult<Horse> pagedHorses)
    {
        return new PagedResponseDto<HorseResponseDto>
        {
            Items = pagedHorses.Items.Select(ToHorseDto).ToList(),
            PageNumber = pagedHorses.PageNumber,
            PageSize = pagedHorses.PageSize,
            TotalCount = pagedHorses.TotalCount,
            TotalPages = (int)Math.Ceiling(pagedHorses.TotalCount / (double)pagedHorses.PageSize)
        };
    }

    public Horse ToEntity(CreateHorseRequestDto request)
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

    public Horse ToEntity(UpdateHorseRequestDto request, int id)
    {
        var horse = ToEntity(new CreateHorseRequestDto
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
        });

        horse.Id = id;
        return horse;
    }
}
