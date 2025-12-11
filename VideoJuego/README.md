# Guerra de Reinos - .NET MAUI

Juego de combate por turnos, pass-and-play para Android. Dos jugadores eligen raza y variante, se mueven por distancia (1-3), atacan o se curan segun sus reglas y gana quien deja al rival en 0 de vida. Tema con acentos morados y soporte de modo claro/oscuro.

## Estructura
- **Models**: configuraciones de personaje (`CharacterConfig`), estados de batalla (`CombatState`, `Combatant`, `CombatActionType`), perfiles (`PlayerProfile`, `MatchRecord`, `BattleResult`, enums de razas/armas).
- **Services**: `CombatService` (reglas de batalla, distancia, dano, curas, sangrado, esquivas), `ImageService` (imagen por raza), `SessionService` (estado en memoria), repositorios SQLite (`DatabaseService`, `PlayerRepository`, `CharacterRepository`, `MatchRepository`, `SaveGameService`).
- **ViewModels**: `MainMenuViewModel`, `PlayerSetupViewModel`, `CharacterSelectionViewModel`, `BattleViewModel`, `MatchSummaryViewModel`, `StatsViewModel` usan `CommunityToolkit.Mvvm`.
- **Views**: paginas XAML vinculadas a cada VM; navegacion via `Shell`.
