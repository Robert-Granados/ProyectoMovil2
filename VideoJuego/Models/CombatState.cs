namespace VideoJuego.Models;

public class CombatState
{
    public IList<Combatant> Combatants { get; set; } = new List<Combatant>();
    public int ActiveIndex { get; set; }
    public int Distance { get; set; } = 0;
    public int TurnNumber { get; set; } = 1;
    public bool IsFinished { get; set; }
    public string? WinnerName { get; set; }
    public List<string> Log { get; set; } = new();

    public Combatant Active => Combatants[ActiveIndex];
    public Combatant Opponent => Combatants[(ActiveIndex + 1) % 2];
}
