using horseappspring26.ViewModels;

namespace horseappspring26.Views;

[QueryProperty(nameof(ReturnTo), "returnTo")]
public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public string ReturnTo
    {
        set
        {
            _viewModel.ReturnToRoute = string.IsNullOrWhiteSpace(value)
                ? "//horses"
                : Uri.UnescapeDataString(value);
        }
    }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
