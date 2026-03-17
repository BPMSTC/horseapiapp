using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using horseappspring26.Models;
using horseappspring26.Services;
using Microsoft.Extensions.Logging;

namespace horseappspring26.ViewModels;

public partial class HorseEditViewModel(IHorseApiService horseApiService, IAuthService authService, ILogger<HorseEditViewModel> logger) : ObservableObject
{
    private int? _horseId;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _registrationNumber = string.Empty;

    [ObservableProperty]
    private DateTime _dateOfBirth = DateTime.UtcNow.AddYears(-3);

    [ObservableProperty]
    private Gender _gender = Gender.Stallion;

    [ObservableProperty]
    private string _color = string.Empty;

    [ObservableProperty]
    private string _sire = string.Empty;

    [ObservableProperty]
    private string _dam = string.Empty;

    [ObservableProperty]
    private string _breederName = string.Empty;

    [ObservableProperty]
    private string _pictureUrl = string.Empty;

    [ObservableProperty]
    private int _totalRacesRun;

    [ObservableProperty]
    private int _wins;

    [ObservableProperty]
    private int _places;

    [ObservableProperty]
    private int _shows;

    [ObservableProperty]
    private decimal _careerEarnings;

    [ObservableProperty]
    private string _currentOwner = string.Empty;

    [ObservableProperty]
    private string _trainer = string.Empty;

    public IReadOnlyList<Gender> GenderOptions { get; } = Enum.GetValues<Gender>();

    public string PageTitle => IsEditMode ? "Edit Horse" : "Add Horse";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(PageTitle));
    }

    [RelayCommand]
    private async Task LoadHorseAsync(int id)
    {
        if (IsBusy)
        {
            return;
        }

        if (!authService.IsLoggedIn)
        {
            var returnTo = Uri.EscapeDataString($"horseedit?id={id}");
            await Shell.Current.GoToAsync($"//login?returnTo={returnTo}");
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            IsEditMode = true;
            _horseId = id;

            var horse = await horseApiService.GetHorseByIdAsync(id);
            if (horse is null)
            {
                ErrorMessage = "Horse not found.";
                return;
            }

            Name = horse.Name;
            RegistrationNumber = horse.RegistrationNumber;
            DateOfBirth = horse.DateOfBirth;
            Gender = horse.Gender;
            Color = horse.Color;
            Sire = horse.Sire ?? string.Empty;
            Dam = horse.Dam ?? string.Empty;
            BreederName = horse.BreederName ?? string.Empty;
            PictureUrl = horse.PictureUrl ?? string.Empty;
            TotalRacesRun = horse.TotalRacesRun;
            Wins = horse.Wins;
            Places = horse.Places;
            Shows = horse.Shows;
            CareerEarnings = horse.CareerEarnings;
            CurrentOwner = horse.CurrentOwner ?? string.Empty;
            Trainer = horse.Trainer ?? string.Empty;
        }
        catch (ApiException ex)
        {
            logger.LogWarning(ex, "API error while loading horse for edit. ID: {HorseId}", id);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while loading horse for edit. ID: {HorseId}", id);
            ErrorMessage = "Unable to load horse for editing right now.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void PrepareForCreate()
    {
        IsEditMode = false;
        _horseId = null;
        ErrorMessage = string.Empty;

        Name = string.Empty;
        RegistrationNumber = string.Empty;
        DateOfBirth = DateTime.UtcNow.AddYears(-3);
        Gender = Gender.Stallion;
        Color = string.Empty;
        Sire = string.Empty;
        Dam = string.Empty;
        BreederName = string.Empty;
        PictureUrl = string.Empty;
        TotalRacesRun = 0;
        Wins = 0;
        Places = 0;
        Shows = 0;
        CareerEarnings = 0;
        CurrentOwner = string.Empty;
        Trainer = string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (!authService.IsLoggedIn)
        {
            var target = IsEditMode && _horseId.HasValue ? $"horseedit?id={_horseId.Value}" : "horseedit";
            var returnTo = Uri.EscapeDataString(target);
            await Shell.Current.GoToAsync($"//login?returnTo={returnTo}");
            return;
        }

        var validationError = Validate();
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            ErrorMessage = validationError;
            return;
        }

        var request = new HorseUpsertRequest
        {
            Name = Name.Trim(),
            RegistrationNumber = RegistrationNumber.Trim(),
            DateOfBirth = DateOfBirth,
            Gender = Gender,
            Color = Color.Trim(),
            Sire = ToNullable(Sire),
            Dam = ToNullable(Dam),
            BreederName = ToNullable(BreederName),
            PictureUrl = ToNullable(PictureUrl),
            TotalRacesRun = TotalRacesRun,
            Wins = Wins,
            Places = Places,
            Shows = Shows,
            CareerEarnings = CareerEarnings,
            CurrentOwner = ToNullable(CurrentOwner),
            Trainer = ToNullable(Trainer)
        };

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (IsEditMode && _horseId.HasValue)
            {
                await horseApiService.UpdateHorseAsync(_horseId.Value, request);
            }
            else
            {
                await horseApiService.CreateHorseAsync(request);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (ApiException ex)
        {
            logger.LogWarning(ex, "API error while saving horse.");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while saving horse.");
            ErrorMessage = "Unable to save horse right now.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private string Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return "Name is required.";
        }

        if (string.IsNullOrWhiteSpace(RegistrationNumber))
        {
            return "Registration number is required.";
        }

        if (string.IsNullOrWhiteSpace(Color))
        {
            return "Color is required.";
        }

        if (TotalRacesRun < 0 || Wins < 0 || Places < 0 || Shows < 0)
        {
            return "Race statistics cannot be negative.";
        }

        if (CareerEarnings < 0)
        {
            return "Career earnings cannot be negative.";
        }

        return string.Empty;
    }

    private static string? ToNullable(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
