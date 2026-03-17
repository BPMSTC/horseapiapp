using System.ComponentModel.DataAnnotations;

namespace horseapispring26.Models.DTOs;

public class CreateHorseRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    [MaxLength(100)]
    public string Color { get; set; } = string.Empty;

    public string? Sire { get; set; }
    public string? Dam { get; set; }
    public string? BreederName { get; set; }
    public string? PictureUrl { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalRacesRun { get; set; }

    [Range(0, int.MaxValue)]
    public int Wins { get; set; }

    [Range(0, int.MaxValue)]
    public int Places { get; set; }

    [Range(0, int.MaxValue)]
    public int Shows { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CareerEarnings { get; set; }

    public string? CurrentOwner { get; set; }
    public string? Trainer { get; set; }
}
