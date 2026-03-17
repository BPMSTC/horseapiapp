using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using horseappspring26.Configuration;
using horseappspring26.Models;
using horseappspring26.Services;
using Microsoft.Extensions.Logging;

namespace horseappspring26.ViewModels;

public partial class HorseDetailViewModel(
    IHorseApiService horseApiService,
    IAuthService authService,
    ApiSettings apiSettings,
    ILogger<HorseDetailViewModel> logger) : ObservableObject
{
    private int _horseId;

    [ObservableProperty]
    private Horse? _horse;

    [ObservableProperty]
    private string? _horseImageUrl;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public bool CanDelete => authService.IsAdmin;

    partial void OnHorseChanged(Horse? value)
    {
        HorseImageUrl = BuildImageUrl(value?.PictureUrl);
    }

    [RelayCommand]
    private async Task LoadHorseAsync(int id)
    {
        if (IsBusy)
        {
            return;
        }

        _horseId = id;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            Horse = await horseApiService.GetHorseByIdAsync(id);
            if (Horse is null)
            {
                ErrorMessage = "Horse not found.";
            }

            OnPropertyChanged(nameof(CanDelete));
        }
        catch (ApiException ex)
        {
            logger.LogWarning(ex, "API error while loading horse detail for ID {HorseId}", id);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while loading horse detail for ID {HorseId}", id);
            ErrorMessage = "Unable to load horse details right now.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string? BuildImageUrl(string? pictureUrl)
    {
        if (string.IsNullOrWhiteSpace(pictureUrl))
        {
            return null;
        }

        var normalized = pictureUrl.Trim().Replace('\\', '/');

        if (Uri.TryCreate(normalized, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        if (!Uri.TryCreate(apiSettings.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            return normalized;
        }

        return new Uri(baseUri, normalized.TrimStart('/')).ToString();
    }

    [RelayCommand]
    private async Task EditHorseAsync()
    {
        if (_horseId <= 0)
        {
            return;
        }

        if (!authService.IsLoggedIn)
        {
            var returnTo = Uri.EscapeDataString($"horseedit?id={_horseId}");
            await Shell.Current.GoToAsync($"//login?returnTo={returnTo}");
            return;
        }

        await Shell.Current.GoToAsync($"horseedit?id={_horseId}");
    }

    [RelayCommand]
    private async Task DeleteHorseAsync()
    {
        if (_horseId <= 0)
        {
            return;
        }

        if (!authService.IsLoggedIn)
        {
            var returnTo = Uri.EscapeDataString($"horsedetail?id={_horseId}");
            await Shell.Current.GoToAsync($"//login?returnTo={returnTo}");
            return;
        }

        if (!CanDelete)
        {
            ErrorMessage = "Delete requires an Admin account.";
            return;
        }

        var confirmed = await Shell.Current.DisplayAlert("Delete Horse", "Are you sure you want to delete this horse?", "Delete", "Cancel");
        if (!confirmed)
        {
            return;
        }

        try
        {
            ErrorMessage = string.Empty;
            await horseApiService.DeleteHorseAsync(_horseId);
            await Shell.Current.GoToAsync("..");
        }
        catch (ApiException ex)
        {
            logger.LogWarning(ex, "API error while deleting horse ID {HorseId}", _horseId);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while deleting horse ID {HorseId}", _horseId);
            ErrorMessage = "Unable to delete horse right now.";
        }
    }
}
