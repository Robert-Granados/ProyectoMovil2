using System;
using System.Collections.Generic;
using System.Text.Json;

namespace VideoJuego.Models;

public class SavedGame
{
    public int Id { get; set; } = 1;
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public Race Player1Race { get; set; }
    public Race Player2Race { get; set; }
    public HumanWeapon? Player1HumanWeapon { get; set; }
    public HumanWeapon? Player2HumanWeapon { get; set; }
    public ElfElement? Player1ElfElement { get; set; }
    public ElfElement? Player2ElfElement { get; set; }
    public OrcWeapon? Player1OrcWeapon { get; set; }
    public OrcWeapon? Player2OrcWeapon { get; set; }
    public BeastAttack? Player1BeastAttack { get; set; }
    public BeastAttack? Player2BeastAttack { get; set; }
    public double Player1CurrentHp { get; set; }
    public double Player2CurrentHp { get; set; }
    public double Player1MaxHp { get; set; }
    public double Player2MaxHp { get; set; }
    public int Distance { get; set; }
    public int TurnNumber { get; set; }
    public int ActiveIndex { get; set; }
    public bool Player1Bleeding { get; set; }
    public bool Player2Bleeding { get; set; }
    public int Player1BleedingTurnsLeft { get; set; }
    public int Player2BleedingTurnsLeft { get; set; }
    public double Player1PendingHeal { get; set; }
    public double Player2PendingHeal { get; set; }
    public string CurrentTurnPlayerName { get; set; } = string.Empty;
    public DateTime LastSavedAt { get; set; }

    public IList<string> LogEntries { get; set; } = new List<string>();

    public string LogJson
    {
        get => JsonSerializer.Serialize(LogEntries);
        set
        {
            try
            {
                LogEntries = string.IsNullOrWhiteSpace(value)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
            }
            catch
            {
                LogEntries = new List<string>();
            }
        }
    }
}
