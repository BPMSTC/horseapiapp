using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using horseappspring26.Services;
using Microsoft.Extensions.Logging;

namespace horseappspring26.ViewModels;

public partial class LoginViewModel(IAuthService authService, ILogger<LoginViewModel> logger) : ObservableObject
{
    public string ReturnToRoute { get; set; } = "//horses";

    [ObservableProperty]
    private string _email = "admin@horseapi.com";

    [ObservableProperty]
    private string _password = "Admin123!";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            return;
        }

        try
        {
            IsBusy = true;
            var loggedIn = await authService.LoginAsync(Email.Trim(), Password);

            if (!loggedIn)
            {
                ErrorMessage = "Login failed. Check your credentials and API connection.";
                return;
            }

            var route = string.IsNullOrWhiteSpace(ReturnToRoute) ? "//horses" : ReturnToRoute;
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while logging in.");
            ErrorMessage = "Unable to log in right now.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
