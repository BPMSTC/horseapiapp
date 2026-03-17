namespace horseappspring26.Models;

public class HorseUpsertRequest
{
    public string Name { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Color { get; set; } = string.Empty;
    public string? Sire { get; set; }
    public string? Dam { get; set; }
    public string? BreederName { get; set; }
    public string? PictureUrl { get; set; }
    public int TotalRacesRun { get; set; }
    public int Wins { get; set; }
    public int Places { get; set; }
    public int Shows { get; set; }
    public decimal CareerEarnings { get; set; }
    public string? CurrentOwner { get; set; }
    public string? Trainer { get; set; }
}
