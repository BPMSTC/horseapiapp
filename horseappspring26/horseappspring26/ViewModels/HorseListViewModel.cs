using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using horseappspring26.Configuration;
using horseappspring26.Models;
using horseappspring26.Services;
using Microsoft.Extensions.Logging;

namespace horseappspring26.ViewModels;

public partial class HorseListViewModel(
    IHorseApiService horseApiService,
    IAuthService authService,
    ApiSettings apiSettings,
    ILogger<HorseListViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Horse> _horses = [];

    [ObservableProperty]
    private string _searchTerm = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public bool CanDelete => authService.IsAdmin;

    public bool IsLoggedIn => authService.IsLoggedIn;

    public string AuthActionText => IsLoggedIn ? "Logout" : "Login";

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var response = string.IsNullOrWhiteSpace(SearchTerm)
                ? await horseApiService.GetHorsesAsync(apiSettings.DefaultPageNumber, apiSettings.DefaultPageSize)
                : await horseApiService.SearchHorsesAsync(SearchTerm.Trim(), apiSettings.DefaultPageNumber, apiSettings.DefaultPageSize);

            Horses = new ObservableCollection<Horse>(response.Items);
            OnPropertyChanged(nameof(CanDelete));
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(AuthActionText));
        }
        catch (ApiException ex)
        {
            logger.LogWarning(ex, "API error while loading horses.");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while loading horses.");
            ErrorMessage = "Unable to load horses right now.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task AddHorseAsync()
    {
        if (!authService.IsLoggedIn)
        {
            var returnTo = Uri.EscapeDataString("horseedit");
            await Shell.Current.GoToAsync($"//login?returnTo={returnTo}");
            return;
        }

        await Shell.Current.GoToAsync("horseedit");
    }

    [RelayCommand]
    private async Task OpenHorseAsync(Horse? horse)
    {
        if (horse is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"horsedetail?id={horse.Id}");
    }

    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        if (!authService.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("//login");
            return;
        }

        authService.Logout();
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(AuthActionText));
    }
}
