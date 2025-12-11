using VideoJuego.ViewModels;

namespace VideoJuego.Views;

public partial class MainMenuPage : ContentPage
{
    private readonly MainMenuViewModel _viewModel;

    public MainMenuPage(MainMenuViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
