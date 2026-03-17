using horseappspring26.ViewModels;

namespace horseappspring26.Views;

[QueryProperty(nameof(HorseId), "id")]
public partial class HorseEditPage : ContentPage
{
    private readonly HorseEditViewModel _viewModel;

    private int _horseId;

    public int HorseId
    {
        get => _horseId;
        set
        {
            _horseId = value;
            _ = LoadForEditAsync(value);
        }
    }

    public HorseEditPage(HorseEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_horseId <= 0)
        {
            _viewModel.PrepareForCreate();
        }
    }

    private async Task LoadForEditAsync(int horseId)
    {
        if (horseId > 0)
        {
            await _viewModel.LoadHorseCommand.ExecuteAsync(horseId);
        }
    }
}
