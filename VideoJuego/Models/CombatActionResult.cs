namespace VideoJuego.Models;

public class CombatActionResult
{
    public string Message { get; set; } = string.Empty;
    public bool TurnConsumed { get; set; }
    public bool BattleEnded { get; set; }
    public Combatant? Winner { get; set; }
}
