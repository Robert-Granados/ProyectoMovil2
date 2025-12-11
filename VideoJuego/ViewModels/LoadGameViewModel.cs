using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VideoJuego.Models;
using VideoJuego.Services;

namespace VideoJuego.ViewModels;

public partial class LoadGameViewModel : BaseViewModel
{
    private readonly ISaveGameService _saveGameService;
    private readonly ISessionService _sessionService;
    private readonly IImageService _imageService;

    public LoadGameViewModel(ISaveGameService saveGameService, ISessionService sessionService, IImageService imageService)
    {
        _saveGameService = saveGameService;
        _sessionService = sessionService;
        _imageService = imageService;
        Title = "Cargar partida";
    }

    public ObservableCollection<SavedGame> SavedGames { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task LoadSavesAsync()
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;
        try
        {
            SavedGames.Clear();
            var saves = await _saveGameService.GetAllAsync();
            foreach (var save in saves.OrderByDescending(s => s.LastSavedAt))
            {
                SavedGames.Add(save);
            }

            if (SavedGames.Count == 0)
            {
                await Shell.Current.DisplayAlert("Sin partidas", "No hay partidas guardadas para cargar.", "OK");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadSelectedSaveAsync(SavedGame? save)
    {
        if (save is null)
        {
            return;
        }

        var config1 = BuildCharacterConfig(save.Player1Race, save.Player1HumanWeapon, save.Player1ElfElement, save.Player1OrcWeapon, save.Player1BeastAttack);
        var config2 = BuildCharacterConfig(save.Player2Race, save.Player2HumanWeapon, save.Player2ElfElement, save.Player2OrcWeapon, save.Player2BeastAttack);

        _sessionService.Player1Name = save.Player1Name;
        _sessionService.Player2Name = save.Player2Name;
        _sessionService.Player1Character = config1;
        _sessionService.Player2Character = config2;
        _sessionService.RestoredCombatState = BuildStateFromSave(save, config1, config2);

        await Shell.Current.GoToAsync("BattlePage?mode=Load");
    }

    private CharacterConfig BuildCharacterConfig(Race race, HumanWeapon? human, ElfElement? elf, OrcWeapon? orc, BeastAttack? beast)
    {
        return race switch
        {
            Race.Human => new CharacterConfig { Race = race, HumanWeapon = human ?? HumanWeapon.Shotgun },
            Race.Elf => new CharacterConfig { Race = race, ElfElement = elf ?? ElfElement.Fire },
            Race.Orc => new CharacterConfig { Race = race, OrcWeapon = orc ?? OrcWeapon.Axe },
            Race.Beast => new CharacterConfig { Race = race, BeastAttack = beast ?? BeastAttack.Punches },
            _ => new CharacterConfig { Race = race }
        };
    }

    private CombatState BuildStateFromSave(SavedGame save, CharacterConfig config1, CharacterConfig config2)
    {
        var combatant1 = new Combatant
        {
            PlayerName = save.Player1Name,
            Config = config1,
            MaxHealth = save.Player1MaxHp,
            CurrentHealth = save.Player1CurrentHp,
            BleedTurnsLeft = save.Player1BleedingTurnsLeft,
            PendingHealNextTurn = save.Player1PendingHeal,
            ImageResource = _imageService.GetImageForCharacter(config1)
        };

        var combatant2 = new Combatant
        {
            PlayerName = save.Player2Name,
            Config = config2,
            MaxHealth = save.Player2MaxHp,
            CurrentHealth = save.Player2CurrentHp,
            BleedTurnsLeft = save.Player2BleedingTurnsLeft,
            PendingHealNextTurn = save.Player2PendingHeal,
            ImageResource = _imageService.GetImageForCharacter(config2)
        };

        var activeIndex = Math.Clamp(save.ActiveIndex, 0, 1);
        if (!string.IsNullOrWhiteSpace(save.CurrentTurnPlayerName))
        {
            if (string.Equals(save.CurrentTurnPlayerName, save.Player2Name, StringComparison.OrdinalIgnoreCase))
            {
                activeIndex = 1;
            }
            else if (string.Equals(save.CurrentTurnPlayerName, save.Player1Name, StringComparison.OrdinalIgnoreCase))
            {
                activeIndex = 0;
            }
        }

        return new CombatState
        {
            Combatants = new List<Combatant> { combatant1, combatant2 },
            ActiveIndex = activeIndex,
            Distance = save.Distance,
            TurnNumber = Math.Max(1, save.TurnNumber),
            Log = save.LogEntries.ToList()
        };
    }
}
