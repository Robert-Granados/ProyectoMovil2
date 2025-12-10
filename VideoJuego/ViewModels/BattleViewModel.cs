using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoJuego.Models;
using VideoJuego.Services;
using Microsoft.Maui.Controls;

namespace VideoJuego.ViewModels
{

    [QueryProperty(nameof(Mode), "mode")]
    public partial class BattleViewModel : BaseViewModel
    {
        private readonly ICombatService _combatService;
        private readonly ISessionService _sessionService;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ISaveGameService _saveGameService;
        private CombatState? _state;
        private bool _initialized;
        private string _mode = "New";

        public BattleViewModel(
            ICombatService combatService,
            ISessionService sessionService,
            IPlayerRepository playerRepository,
            ICharacterRepository characterRepository,
            IMatchRepository matchRepository,
            ISaveGameService saveGameService)
        {
            _combatService = combatService;
            _sessionService = sessionService;
            _playerRepository = playerRepository;
            _characterRepository = characterRepository;
            _matchRepository = matchRepository;
            _saveGameService = saveGameService;
            Title = "Combate";
        }

        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value ?? "New";
                ResetLocalState();
            }
        }

        public ObservableCollection<string> Log { get; } = new();

        [ObservableProperty]
        private string activePlayerName = string.Empty;

        [ObservableProperty]
        private string activeImage = string.Empty;

        [ObservableProperty]
        private double player1Health;

        [ObservableProperty]
        private double player2Health;

        [ObservableProperty]
        private double player1MaxHealth;

        [ObservableProperty]
        private double player2MaxHealth;

        [ObservableProperty]
        private int distance;

        [ObservableProperty]
        private double player1HealthRatio;

        [ObservableProperty]
        private double player2HealthRatio;

        [ObservableProperty]
        private string player1HealthText = string.Empty;

        [ObservableProperty]
        private string player2HealthText = string.Empty;

        [ObservableProperty]
        private string player1Image = string.Empty;

        [ObservableProperty]
        private string player2Image = string.Empty;

        [ObservableProperty]
        private int turnNumber;

        [ObservableProperty]
        private string attackLabel = "Atacar";

        public string Player1Name => _sessionService.Player1Name;
        public string Player2Name => _sessionService.Player2Name;

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (_initialized)
            {
                return;
            }

            var isLoadMode = string.Equals(Mode, "Load", StringComparison.OrdinalIgnoreCase);

            if (isLoadMode)
            {
                if (_sessionService.RestoredCombatState is null)
                {
                    await Shell.Current.DisplayAlert("Sin partida", "Selecciona una partida a cargar.", "OK");
                    await Shell.Current.GoToAsync("//MainMenuPage");
                    return;
                }

                _state = _sessionService.RestoredCombatState;
                _initialized = true;
                SyncState();
                return;
            }

            // New battle flow
            _sessionService.RestoredCombatState = null;

            if (_sessionService.Player1Character is null || _sessionService.Player2Character is null)
            {
                await Shell.Current.DisplayAlert("Faltan datos", "Configura los personajes antes de combatir.", "OK");
                await Shell.Current.GoToAsync("PlayerSetupPage");
                return;
            }

            if (_sessionService.RestoredCombatState is not null)
            {
                _state = _sessionService.RestoredCombatState;
                _initialized = true;
                SyncState();
                return;
            }

            _state = _combatService.StartBattle(Player1Name, _sessionService.Player1Character, Player2Name, _sessionService.Player2Character);
            _initialized = true;
            _state.Log.Add("Inicio: Comienza el combate!");
            SyncState();
        }

        [RelayCommand]
        private async Task AdvanceAsync() => await ExecuteActionAsync(CombatActionType.Advance);

        [RelayCommand]
        private async Task RetreatAsync() => await ExecuteActionAsync(CombatActionType.Retreat);

        [RelayCommand]
        private async Task AttackAsync()
        {
            var action = GetAttackActionForActive();
            await ExecuteActionAsync(action);
        }

        [RelayCommand]
        private async Task HealAsync() => await ExecuteActionAsync(CombatActionType.Heal);

        [RelayCommand]
        private async Task SaveGameAsync()
        {
            if (_state is null)
            {
                return;
            }

            var savePayload = BuildSavedGame(_state);
            await _saveGameService.SaveGameAsync(savePayload);
            _state.Log.Add("Partida guardada correctamente");
            Log.Add("Partida guardada correctamente");
        }

        [RelayCommand]
        private async Task RestartGameAsync()
        {
            await _saveGameService.DeleteSavedGameAsync();
            _sessionService.Clear();
            _state = null;
            _initialized = false;
            Log.Clear();
            await Shell.Current.GoToAsync("PlayerSetupPage");
        }

        [RelayCommand]
        private async Task ExitToMainMenuAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Salir al menú",
                "¿Deseas salir al menú principal? La partida actual se perderá si no la guardaste.",
                "Sí, salir",
                "Cancelar");

            if (!confirm)
            {
                return;
            }

            ClearSessionState();
            await Shell.Current.GoToAsync("//MainMenuPage");
        }

        private async Task ExecuteActionAsync(CombatActionType action)
        {
            if (_state is null)
            {
                return;
            }

            var result = _combatService.PerformAction(_state, action);
            SyncState();

            if (result.BattleEnded || _state.IsFinished)
            {
                await FinalizeMatchAsync(result.Winner ?? _state.Combatants.FirstOrDefault(c => c.IsAlive));
            }
        }

        private void SyncState()
        {
            if (_state is null)
            {
                return;
            }

            Player1Health = Math.Max(0, Math.Round(_state.Combatants[0].CurrentHealth, 1));
            Player2Health = Math.Max(0, Math.Round(_state.Combatants[1].CurrentHealth, 1));
            Player1MaxHealth = _state.Combatants[0].MaxHealth;
            Player2MaxHealth = _state.Combatants[1].MaxHealth;
            Player1HealthRatio = Player1MaxHealth <= 0 ? 0 : Player1Health / Player1MaxHealth;
            Player2HealthRatio = Player2MaxHealth <= 0 ? 0 : Player2Health / Player2MaxHealth;
            Player1HealthText = $"{Player1Name} - Vida: {Player1Health:0}/{Player1MaxHealth:0}";
            Player2HealthText = $"{Player2Name} - Vida: {Player2Health:0}/{Player2MaxHealth:0}";
            Player1Image = _state.Combatants[0].ImageResource;
            Player2Image = _state.Combatants[1].ImageResource;
            Distance = _state.Distance;
            TurnNumber = _state.TurnNumber;
            ActivePlayerName = _state.Active.PlayerName;
            ActiveImage = _state.Active.ImageResource;
            AttackLabel = BuildAttackLabel(_state.Active.Config);

            // Sync log from state in case effects added messages.
            if (Log.Count == 0 && _state.Log.Count > 0)
            {
                foreach (var entry in _state.Log)
                {
                    Log.Add(entry);
                }
            }

            while (Log.Count < _state.Log.Count)
            {
                Log.Add(_state.Log[Log.Count]);
            }
        }

        private string BuildAttackLabel(CharacterConfig config)
        {
            return config.Race switch
            {
                Race.Human => config.HumanWeapon == HumanWeapon.SniperRifle ? "Disparar rifle" : "Disparar escopeta",
                Race.Elf => config.ElfElement switch
                {
                    ElfElement.Fire => "Hechizo de fuego",
                    ElfElement.Earth => "Hechizo de tierra",
                    ElfElement.Air => "Hechizo de aire",
                    ElfElement.Water => "Hechizo de agua",
                    _ => "Hechizo"
                },
                Race.Orc => config.OrcWeapon == OrcWeapon.Hammer ? "Golpe de martillo" : "Golpe de hacha",
                Race.Beast => config.BeastAttack == BeastAttack.Sword ? "Ataque con espada" : "Golpe de puños",
                _ => "Atacar"
            };
        }

        private CombatActionType GetAttackActionForActive()
        {
            if (_state is null)
            {
                return CombatActionType.HumanShotgun;
            }

            var config = _state.Active.Config;
            return config.Race switch
            {
                Race.Human => config.HumanWeapon == HumanWeapon.SniperRifle
                    ? CombatActionType.HumanSniper
                    : CombatActionType.HumanShotgun,
                Race.Elf => CombatActionType.ElfSpell,
                Race.Orc => config.OrcWeapon == OrcWeapon.Hammer
                    ? CombatActionType.OrcHammer
                    : CombatActionType.OrcAxe,
                Race.Beast => config.BeastAttack == BeastAttack.Sword
                    ? CombatActionType.BeastSword
                    : CombatActionType.BeastPunches,
                _ => CombatActionType.HumanShotgun
            };
        }

        private async Task FinalizeMatchAsync(Combatant? winner)
        {
            if (_state is null)
            {
                return;
            }

            var wasDraw = winner is null;
            var winnerName = winner?.PlayerName ?? "Empate";

            var player1Profile = await _playerRepository.GetOrCreateAsync(Player1Name);
            var player2Profile = await _playerRepository.GetOrCreateAsync(Player2Name);

            if (winner is not null)
            {
                if (winner.PlayerName == Player1Name)
                {
                    await _playerRepository.UpdateStatsAsync(player1Profile.Id, true, false);
                    await _playerRepository.UpdateStatsAsync(player2Profile.Id, false, false);
                }
                else
                {
                    await _playerRepository.UpdateStatsAsync(player2Profile.Id, true, false);
                    await _playerRepository.UpdateStatsAsync(player1Profile.Id, false, false);
                }
            }
            else
            {
                await _playerRepository.UpdateStatsAsync(player1Profile.Id, false, true);
                await _playerRepository.UpdateStatsAsync(player2Profile.Id, false, true);
            }

            await _characterRepository.SaveOrUpdateAsync(player1Profile.Id, _sessionService.Player1Character!);
            await _characterRepository.SaveOrUpdateAsync(player2Profile.Id, _sessionService.Player2Character!);

            await _matchRepository.AddAsync(new MatchRecord
            {
                Date = DateTime.Now,
                Player1Id = player1Profile.Id,
                Player2Id = player2Profile.Id,
                WinnerPlayerId = winner is null ? null : winner.PlayerName == Player1Name ? player1Profile.Id : player2Profile.Id,
                WasDraw = wasDraw,
                Player1Name = Player1Name,
                Player2Name = Player2Name,
                Player1Race = _sessionService.Player1Character!.Race,
                Player2Race = _sessionService.Player2Character!.Race,
                Player1Variant = _sessionService.Player1Character.Describe(),
                Player2Variant = _sessionService.Player2Character.Describe()
            });

            _sessionService.LastResult = new BattleResult
            {
                WinnerName = winnerName,
                WinnerHealth = winner?.CurrentHealth ?? 0,
                WinnerCharacter = winner?.Config ?? new CharacterConfig(),
                Player1Character = _sessionService.Player1Character!,
                Player2Character = _sessionService.Player2Character!,
                Player1Name = Player1Name,
                Player2Name = Player2Name,
                Turns = _state.TurnNumber,
                WasDraw = wasDraw
            };

            await _saveGameService.DeleteSavedGameAsync();
            _sessionService.RestoredCombatState = null;

            await Shell.Current.GoToAsync("MatchSummaryPage");
        }

        private SavedGame BuildSavedGame(CombatState state)
        {
            var player1 = state.Combatants[0];
            var player2 = state.Combatants[1];

            return new SavedGame
            {
                Id = 1,
                Player1Name = player1.PlayerName,
                Player2Name = player2.PlayerName,
                Player1Race = player1.Config.Race,
                Player2Race = player2.Config.Race,
                Player1HumanWeapon = player1.Config.HumanWeapon,
                Player2HumanWeapon = player2.Config.HumanWeapon,
                Player1ElfElement = player1.Config.ElfElement,
                Player2ElfElement = player2.Config.ElfElement,
                Player1OrcWeapon = player1.Config.OrcWeapon,
                Player2OrcWeapon = player2.Config.OrcWeapon,
                Player1BeastAttack = player1.Config.BeastAttack,
                Player2BeastAttack = player2.Config.BeastAttack,
                Player1CurrentHp = player1.CurrentHealth,
                Player2CurrentHp = player2.CurrentHealth,
                Player1MaxHp = player1.MaxHealth,
                Player2MaxHp = player2.MaxHealth,
                Distance = state.Distance,
                TurnNumber = state.TurnNumber,
                ActiveIndex = state.ActiveIndex,
                Player1Bleeding = player1.BleedTurnsLeft > 0,
                Player2Bleeding = player2.BleedTurnsLeft > 0,
                Player1BleedingTurnsLeft = player1.BleedTurnsLeft,
                Player2BleedingTurnsLeft = player2.BleedTurnsLeft,
                Player1PendingHeal = player1.PendingHealNextTurn,
                Player2PendingHeal = player2.PendingHealNextTurn,
                CurrentTurnPlayerName = state.Active.PlayerName,
                LastSavedAt = DateTime.Now,
                LogEntries = state.Log.ToList()
            };
        }

        private void ResetLocalState()
        {
            _state = null;
            _initialized = false;
            Log.Clear();
        }

        private void ClearSessionState()
        {
            ResetLocalState();
            _sessionService.Clear();
        }
    }

}
