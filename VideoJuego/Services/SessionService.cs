using VideoJuego.Models;

namespace VideoJuego.Services;

public class SessionService : ISessionService
{
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public CharacterConfig? Player1Character { get; set; }
    public CharacterConfig? Player2Character { get; set; }
    public BattleResult? LastResult { get; set; }
    public CombatState? RestoredCombatState { get; set; }

    public void Clear()
    {
        Player1Name = string.Empty;
        Player2Name = string.Empty;
        Player1Character = null;
        Player2Character = null;
        LastResult = null;
        RestoredCombatState = null;
    }
}
