namespace VideoJuego.Models;

//clase para representar el resultado de una acción de combate
public class CombatActionResult
{
    public string Message { get; set; } = string.Empty;
    public bool TurnConsumed { get; set; }
    public bool BattleEnded { get; set; }
    public Combatant? Winner { get; set; }
}
