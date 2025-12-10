using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoJuego.Models;
using VideoJuego.Services;

namespace VideoJuego.ViewModels;
    public partial class CharacterSelectionViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;

        public CharacterSelectionViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            Title = "Seleccionar personajes";
            SelectedRace1 = Race.Human;
            SelectedRace2 = Race.Human;
        }

        public IList<Race> Races { get; } = Enum.GetValues<Race>().ToList();
        public IList<HumanWeapon> HumanWeapons { get; } = Enum.GetValues<HumanWeapon>().ToList();
        public IList<ElfElement> ElfElements { get; } = Enum.GetValues<ElfElement>().ToList();
        public IList<OrcWeapon> OrcWeapons { get; } = Enum.GetValues<OrcWeapon>().ToList();
        public IList<BeastAttack> BeastAttacks { get; } = Enum.GetValues<BeastAttack>().ToList();

        public bool IsHuman1 => SelectedRace1 == Race.Human;
        public bool IsElf1 => SelectedRace1 == Race.Elf;
        public bool IsOrc1 => SelectedRace1 == Race.Orc;
        public bool IsBeast1 => SelectedRace1 == Race.Beast;
        public bool IsHuman2 => SelectedRace2 == Race.Human;
        public bool IsElf2 => SelectedRace2 == Race.Elf;
        public bool IsOrc2 => SelectedRace2 == Race.Orc;
        public bool IsBeast2 => SelectedRace2 == Race.Beast;

        [ObservableProperty]
        private Race selectedRace1;

        [ObservableProperty]
        private Race selectedRace2;

        [ObservableProperty]
        private HumanWeapon selectedHumanWeapon1 = HumanWeapon.Shotgun;

        [ObservableProperty]
        private HumanWeapon selectedHumanWeapon2 = HumanWeapon.Shotgun;

        [ObservableProperty]
        private ElfElement selectedElfElement1 = ElfElement.Fire;

        [ObservableProperty]
        private ElfElement selectedElfElement2 = ElfElement.Fire;

        [ObservableProperty]
        private OrcWeapon selectedOrcWeapon1 = OrcWeapon.Axe;

        [ObservableProperty]
        private OrcWeapon selectedOrcWeapon2 = OrcWeapon.Axe;

        [ObservableProperty]
        private BeastAttack selectedBeastAttack1 = BeastAttack.Punches;

        [ObservableProperty]
        private BeastAttack selectedBeastAttack2 = BeastAttack.Punches;

        public string Player1Name => _sessionService.Player1Name;
        public string Player2Name => _sessionService.Player2Name;

        [RelayCommand]
        private async Task StartBattleAsync()
        {
            if (string.IsNullOrWhiteSpace(Player1Name) || string.IsNullOrWhiteSpace(Player2Name))
            {
                await Shell.Current.DisplayAlert("Faltan nombres", "Configura los jugadores antes de elegir personajes.", "OK");
                return;
            }

            if (!ValidateSelections(out var message))
            {
                await Shell.Current.DisplayAlert("Datos incompletos", message, "OK");
                return;
            }

            _sessionService.Player1Character = BuildCharacterConfig(SelectedRace1, 1);
            _sessionService.Player2Character = BuildCharacterConfig(SelectedRace2, 2);

            await Shell.Current.GoToAsync("BattlePage?mode=New");
        }

        private CharacterConfig BuildCharacterConfig(Race race, int playerIndex)
        {
            return race switch
            {
                Race.Human => new CharacterConfig { Race = race, HumanWeapon = playerIndex == 1 ? SelectedHumanWeapon1 : SelectedHumanWeapon2 },
                Race.Elf => new CharacterConfig { Race = race, ElfElement = playerIndex == 1 ? SelectedElfElement1 : SelectedElfElement2 },
                Race.Orc => new CharacterConfig { Race = race, OrcWeapon = playerIndex == 1 ? SelectedOrcWeapon1 : SelectedOrcWeapon2 },
                Race.Beast => new CharacterConfig { Race = race, BeastAttack = playerIndex == 1 ? SelectedBeastAttack1 : SelectedBeastAttack2 },
                _ => new CharacterConfig { Race = race }
            };
        }

        partial void OnSelectedRace1Changed(Race value)
        {
            OnPropertyChanged(nameof(IsHuman1));
            OnPropertyChanged(nameof(IsElf1));
            OnPropertyChanged(nameof(IsOrc1));
            OnPropertyChanged(nameof(IsBeast1));
        }

        partial void OnSelectedRace2Changed(Race value)
        {
            OnPropertyChanged(nameof(IsHuman2));
            OnPropertyChanged(nameof(IsElf2));
            OnPropertyChanged(nameof(IsOrc2));
            OnPropertyChanged(nameof(IsBeast2));
        }

        private bool ValidateSelections(out string message)
        {
            if (SelectedRace1 == Race.Human && !Enum.IsDefined(typeof(HumanWeapon), SelectedHumanWeapon1))
            {
                message = "Jugador 1: elige un arma de humano.";
                return false;
            }

            if (SelectedRace2 == Race.Human && !Enum.IsDefined(typeof(HumanWeapon), SelectedHumanWeapon2))
            {
                message = "Jugador 2: elige un arma de humano.";
                return false;
            }

            if (SelectedRace1 == Race.Elf && !Enum.IsDefined(typeof(ElfElement), SelectedElfElement1))
            {
                message = "Jugador 1: elige elemento de elfo.";
                return false;
            }

            if (SelectedRace2 == Race.Elf && !Enum.IsDefined(typeof(ElfElement), SelectedElfElement2))
            {
                message = "Jugador 2: elige elemento de elfo.";
                return false;
            }

            if (SelectedRace1 == Race.Orc && !Enum.IsDefined(typeof(OrcWeapon), SelectedOrcWeapon1))
            {
                message = "Jugador 1: elige arma de orco.";
                return false;
            }

            if (SelectedRace2 == Race.Orc && !Enum.IsDefined(typeof(OrcWeapon), SelectedOrcWeapon2))
            {
                message = "Jugador 2: elige arma de orco.";
                return false;
            }

            if (SelectedRace1 == Race.Beast && !Enum.IsDefined(typeof(BeastAttack), SelectedBeastAttack1))
            {
                message = "Jugador 1: elige ataque de bestia.";
                return false;
            }

            if (SelectedRace2 == Race.Beast && !Enum.IsDefined(typeof(BeastAttack), SelectedBeastAttack2))
            {
                message = "Jugador 2: elige ataque de bestia.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }

