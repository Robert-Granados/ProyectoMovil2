using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoJuego.Services;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace VideoJuego.ViewModels;

public partial class MainMenuViewModel : BaseViewModel
{
    private readonly ISaveGameService _saveGameService;
    private readonly ISessionService _sessionService;

    public MainMenuViewModel(ISaveGameService saveGameService, ISessionService sessionService)
    {
        _saveGameService = saveGameService;
        _sessionService = sessionService;
        Title = "Menu principal";
    }

    [ObservableProperty]
    private bool hasSavedGame;

    [ObservableProperty]
    private bool isDarkMode;

    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsDarkMode = Preferences.Get("theme", "dark") == "dark";
        HasSavedGame = await _saveGameService.HasSavedGameAsync();
    }

    [RelayCommand]
    private async Task NewGameAsync()
    {
        _sessionService.Clear();
        await Shell.Current.GoToAsync("PlayerSetupPage");
    }

    [RelayCommand]
    private async Task OpenStatsAsync()
    {
        await Shell.Current.GoToAsync("StatsPage");
    }

    [RelayCommand]
    private async Task ContinueGameAsync()
    {
        var hasSave = await _saveGameService.HasSavedGameAsync();
        if (!hasSave)
        {
            HasSavedGame = false;
            await Shell.Current.DisplayAlert("Sin partida", "No se encontro una partida para continuar.", "OK");
            return;
        }

        await Shell.Current.GoToAsync("LoadGamePage");
    }

    [RelayCommand]
    private async Task OpenCreditsAsync()
    {
        await Shell.Current.GoToAsync("CreditsPage");
    }

    [RelayCommand]
    private async Task OpenGuideAsync()
    {
        await Shell.Current.GoToAsync("GuidePage");
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        ApplyTheme(value);
        Preferences.Set("theme", value ? "dark" : "light");
    }

    private void ApplyTheme(bool darkMode)
    {
        Application.Current.UserAppTheme = darkMode ? AppTheme.Dark : AppTheme.Light;
        if (Application.Current is App app)
        {
            app.ApplyThemeDictionary(darkMode ? "dark" : "light");
        }
    }
}

