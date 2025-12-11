using VideoJuego.ViewModels;

namespace VideoJuego.Views;

public partial class LoadGamePage : ContentPage
{
    private readonly LoadGameViewModel _viewModel;

    public LoadGamePage(LoadGameViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSavesCommand.ExecuteAsync(null);
    }
}
