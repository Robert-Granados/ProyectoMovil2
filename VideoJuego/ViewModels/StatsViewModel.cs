using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoJuego.Models;
using VideoJuego.Services;

namespace VideoJuego.ViewModels;

public partial class StatsViewModel : BaseViewModel
{
    private readonly IPlayerRepository _playerRepository;

    public StatsViewModel(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
        Title = "Estadisticas";
    }

    public ObservableCollection<PlayerProfile> Players { get; } = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        var players = await _playerRepository.GetAllAsync();
        Players.Clear();
        foreach (var player in players)
        {
            Players.Add(player);
        }
    }
}
