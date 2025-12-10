using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoJuego.Services;

namespace VideoJuego.ViewModels;

public partial class PlayerSetupViewModel : BaseViewModel
{
    private readonly ISessionService _sessionService;

    public PlayerSetupViewModel(ISessionService sessionService)
    {
        _sessionService = sessionService;
        Title = "Configurar jugadores";
    }

    [ObservableProperty]
    private string player1Name = string.Empty;

    [ObservableProperty]
    private string player2Name = string.Empty;

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(Player1Name) || string.IsNullOrWhiteSpace(Player2Name))
        {
            await Shell.Current.DisplayAlert("Datos incompletos", "Debes ingresar ambos nombres.", "OK");
            return;
        }

        _sessionService.Player1Name = Player1Name.Trim();
        _sessionService.Player2Name = Player2Name.Trim();
        await Shell.Current.GoToAsync("CharacterSelectionPage");
    }
}
