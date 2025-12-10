using VideoJuego.ViewModels;

namespace VideoJuego.Views;

public partial class CharacterSelectionPage : ContentPage
{
    public CharacterSelectionPage(CharacterSelectionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
