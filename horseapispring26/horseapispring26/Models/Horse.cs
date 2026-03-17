namespace horseapispring26.Models
{
    public class Horse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty; // Unique identifier
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Color { get; set; } = string.Empty;
        public string? Sire { get; set; } // Father
        public string? Dam { get; set; } // Mother
        public string? BreederName { get; set; }
        public string? PictureUrl { get; set; } // URL to horse picture
        
        // Race Record and Performance
        public int TotalRacesRun { get; set; }
        public int Wins { get; set; }
        public int Places { get; set; } // 2nd place finishes
        public int Shows { get; set; } // 3rd place finishes
        public decimal CareerEarnings { get; set; }
        
        // Current Team
        public string? CurrentOwner { get; set; }
        public string? Trainer { get; set; }

        // Soft delete flag (Step 1 schema design)
        public bool IsDeleted { get; set; } = false;
        
        // Calculated properties
        public double WinPercentage => TotalRacesRun > 0 ? (double)Wins / TotalRacesRun * 100 : 0;
        public double PlacePercentage => TotalRacesRun > 0 ? (double)(Wins + Places) / TotalRacesRun * 100 : 0;
        public double ShowPercentage => TotalRacesRun > 0 ? (double)(Wins + Places + Shows) / TotalRacesRun * 100 : 0;
        public int GetAge(DateTime utcNow)
        {
            var today = utcNow.Date;
            var birthdayThisYear = DateOfBirth.Date.AddYears(today.Year - DateOfBirth.Year);
            return birthdayThisYear > today ? today.Year - DateOfBirth.Year - 1 : today.Year - DateOfBirth.Year;
        }
    }

    public enum Gender
    {
        Stallion,
        Mare,
        Gelding
    }
}
