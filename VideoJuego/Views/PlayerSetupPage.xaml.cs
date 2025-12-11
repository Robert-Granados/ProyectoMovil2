using VideoJuego.ViewModels;

namespace VideoJuego.Views;

public partial class PlayerSetupPage : ContentPage
{
    public PlayerSetupPage(PlayerSetupViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
