using Microsoft.AspNetCore.Mvc;
using horseapispring26.Models;
using horseapispring26.Models.DTOs;
using horseapispring26.Repositories;
using horseapispring26.Services;
using Microsoft.AspNetCore.Authorization;

namespace horseapispring26.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HorseController(IHorseRepository horseRepository, IHorseMapper horseMapper) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponseDto<HorseResponseDto>>> GetAllHorses([FromQuery] PaginationQueryDto pagination)
    {
        var horses = await horseRepository.GetAllHorsesAsync(pagination.PageNumber, pagination.PageSize);
        return Ok(horseMapper.ToHorseDtoPage(horses));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<HorseResponseDto>> GetHorseById(int id)
    {
        var horse = await horseRepository.GetHorseByIdAsync(id);
        if (horse == null)
        {
            return NotFound($"Horse with ID {id} not found");
        }
        return Ok(horseMapper.ToHorseDto(horse));
    }

    [HttpGet("registration/{registrationNumber}")]
    [AllowAnonymous]
    public async Task<ActionResult<HorseResponseDto>> GetHorseByRegistrationNumber(string registrationNumber)
    {
        var horse = await horseRepository.GetHorseByRegistrationNumberAsync(registrationNumber);
        if (horse == null)
        {
            return NotFound($"Horse with registration number {registrationNumber} not found");
        }
        return Ok(horseMapper.ToHorseDto(horse));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<HorseResponseDto>> CreateHorse([FromBody] CreateHorseRequestDto horse)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdHorse = await horseRepository.CreateHorseAsync(horseMapper.ToEntity(horse));
        return CreatedAtAction(nameof(GetHorseById), new { id = createdHorse.Id }, horseMapper.ToHorseDto(createdHorse));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<HorseResponseDto>> UpdateHorse(int id, [FromBody] UpdateHorseRequestDto horse)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedHorse = await horseRepository.UpdateHorseAsync(id, horseMapper.ToEntity(horse, id));
        if (updatedHorse == null)
        {
            return NotFound($"Horse with ID {id} not found");
        }

        return Ok(horseMapper.ToHorseDto(updatedHorse));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteHorse(int id)
    {
        var deleted = await horseRepository.DeleteHorseAsync(id);
        if (!deleted)
        {
            return NotFound($"Horse with ID {id} not found");
        }

        return NoContent();
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponseDto<HorseResponseDto>>> SearchHorses([FromQuery] string searchTerm, [FromQuery] PaginationQueryDto pagination)
    {
        var horses = await horseRepository.SearchHorsesAsync(searchTerm, pagination.PageNumber, pagination.PageSize);
        return Ok(horseMapper.ToHorseDtoPage(horses));
    }

    [HttpGet("owner/{owner}")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponseDto<HorseResponseDto>>> GetHorsesByOwner(string owner, [FromQuery] PaginationQueryDto pagination)
    {
        var horses = await horseRepository.GetHorsesByOwnerAsync(owner, pagination.PageNumber, pagination.PageSize);
        return Ok(horseMapper.ToHorseDtoPage(horses));
    }

    [HttpGet("trainer/{trainer}")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponseDto<HorseResponseDto>>> GetHorsesByTrainer(string trainer, [FromQuery] PaginationQueryDto pagination)
    {
        var horses = await horseRepository.GetHorsesByTrainerAsync(trainer, pagination.PageNumber, pagination.PageSize);
        return Ok(horseMapper.ToHorseDtoPage(horses));
    }

    [HttpGet("top-earners")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponseDto<HorseResponseDto>>> GetTopEarners([FromQuery] PaginationQueryDto pagination)
    {
        var horses = await horseRepository.GetTopEarnersAsync(pagination.PageNumber, pagination.PageSize);
        return Ok(horseMapper.ToHorseDtoPage(horses));
    }

    [HttpGet("gender/{gender}")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponseDto<HorseResponseDto>>> GetHorsesByGender(Gender gender, [FromQuery] PaginationQueryDto pagination)
    {
        var horses = await horseRepository.GetHorsesByGenderAsync(gender, pagination.PageNumber, pagination.PageSize);
        return Ok(horseMapper.ToHorseDtoPage(horses));
    }
}
