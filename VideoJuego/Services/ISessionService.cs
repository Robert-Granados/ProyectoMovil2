using VideoJuego.Models;

namespace VideoJuego.Services;

public interface ISessionService
{
    string Player1Name { get; set; }
    string Player2Name { get; set; }
    CharacterConfig? Player1Character { get; set; }
    CharacterConfig? Player2Character { get; set; }
    BattleResult? LastResult { get; set; }
    CombatState? RestoredCombatState { get; set; }
    void Clear();
}
