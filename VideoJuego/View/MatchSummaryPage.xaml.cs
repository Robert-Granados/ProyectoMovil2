using VideoJuego.ViewModels;

namespace VideoJuego.Views;

public partial class MatchSummaryPage : ContentPage
{
    private readonly MatchSummaryViewModel _viewModel;

    public MatchSummaryPage(MatchSummaryViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
    }
}
