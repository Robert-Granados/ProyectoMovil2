using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VideoJuego.Models;
using VideoJuego.Services;

namespace VideoJuego.ViewModels;

public partial class MatchSummaryViewModel : BaseViewModel
{
    private readonly ISessionService _sessionService;

    public MatchSummaryViewModel(ISessionService sessionService)
    {
        _sessionService = sessionService;
        Title = "Resumen de partida";
    }

    [ObservableProperty]
    private string winnerName = string.Empty;

    [ObservableProperty]
    private double winnerHealth;

    [ObservableProperty]
    private int turns;

    [ObservableProperty]
    private string player1Info = string.Empty;

    [ObservableProperty]
    private string player2Info = string.Empty;

    [ObservableProperty]
    private string winnerInfo = string.Empty;

    [RelayCommand]
    private Task BackToMenuAsync()
    {
        return Shell.Current.GoToAsync("//MainMenuPage");
    }

    [RelayCommand]
    private Task GoToStatsAsync()
    {
        return Shell.Current.GoToAsync("StatsPage");
    }

    public void Load()
    {
        if (_sessionService.LastResult is null)
        {
            WinnerName = "Sin datos";
            return;
        }

        var result = _sessionService.LastResult;
        WinnerName = result.WinnerName;
        WinnerHealth = result.WinnerHealth;
        Turns = result.Turns;
        Player1Info = $"{result.Player1Name} - {result.Player1Character.Describe()}";
        Player2Info = $"{result.Player2Name} - {result.Player2Character.Describe()}";
        WinnerInfo = result.WasDraw ? "Empate" : $"{result.WinnerName} ganó con {result.WinnerHealth:0} de vida restante";
    }
}
