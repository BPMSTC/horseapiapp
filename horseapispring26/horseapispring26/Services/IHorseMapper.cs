using horseapispring26.Models;
using horseapispring26.Models.DTOs;

namespace horseapispring26.Services;

public interface IHorseMapper
{
    HorseResponseDto ToHorseDto(Horse horse);
    PagedResponseDto<HorseResponseDto> ToHorseDtoPage(PagedResult<Horse> pagedHorses);
    Horse ToEntity(CreateHorseRequestDto request);
    Horse ToEntity(UpdateHorseRequestDto request, int id);
}
