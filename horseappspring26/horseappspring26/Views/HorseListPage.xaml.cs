using horseappspring26.ViewModels;

namespace horseappspring26.Views;

public partial class HorseListPage : ContentPage
{
    private readonly HorseListViewModel _viewModel;

    public HorseListPage(HorseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
