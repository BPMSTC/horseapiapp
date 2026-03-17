using horseappspring26.ViewModels;

namespace horseappspring26.Views;

[QueryProperty(nameof(HorseId), "id")]
public partial class HorseDetailPage : ContentPage
{
    private readonly HorseDetailViewModel _viewModel;

    private int _horseId;

    public int HorseId
    {
        get => _horseId;
        set
        {
            _horseId = value;
            _ = LoadAsync(value);
        }
    }

    public HorseDetailPage(HorseDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async Task LoadAsync(int horseId)
    {
        await _viewModel.LoadHorseCommand.ExecuteAsync(horseId);
    }
}
