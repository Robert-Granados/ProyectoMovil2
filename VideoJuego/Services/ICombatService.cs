using VideoJuego.Models;

namespace VideoJuego.Services;

public interface ICombatService
{
    CombatState StartBattle(string player1, CharacterConfig config1, string player2, CharacterConfig config2);
    CombatActionResult PerformAction(CombatState state, CombatActionType action);
}
