# Guerra de Reinos - .NET MAUI

Juego de combate por turnos, pass-and-play para Android. Dos jugadores eligen raza y variante, se mueven por distancia (1-3), atacan o se curan segun sus reglas y gana quien deja al rival en 0 de vida. Tema con acentos morados y soporte de modo claro/oscuro.

## Estructura
- **Models**: configuraciones de personaje (`CharacterConfig`), estados de batalla (`CombatState`, `Combatant`, `CombatActionType`), perfiles (`PlayerProfile`, `MatchRecord`, `BattleResult`, enums de razas/armas).
- **Services**: `CombatService` (reglas de batalla, distancia, dano, curas, sangrado, esquivas), `ImageService` (imagen por raza), `SessionService` (estado en memoria), repositorios SQLite (`DatabaseService`, `PlayerRepository`, `CharacterRepository`, `MatchRepository`, `SaveGameService`).
- **ViewModels**: `MainMenuViewModel`, `PlayerSetupViewModel`, `CharacterSelectionViewModel`, `BattleViewModel`, `MatchSummaryViewModel`, `StatsViewModel` usan `CommunityToolkit.Mvvm`.
- **Views**: paginas XAML vinculadas a cada VM; navegacion via `Shell`.

## Logica de batalla (resumen)
- **Distancia**: entero 0..2 (0=cuerpo a cuerpo, 2=lejano). Avanzar reduce, Retroceder aumenta. Solo rifle francotirador y hechizo de Aire funcionan cuando distancia > 0; rifle en 2 hace 10-20 de dano, Aire a distancia suma 5-15%.
- **Vida inicial**: 100; elfo de Agua inicia con 115.
- **Humanos**: escopeta dano 1-5 + bonificacion 0-2 (solo distancia 0); rifle 1-5 en melee, 10-20 en distancia 2. Curar 41-49% de vida perdida.
- **Elfos**: magia por elemento (Fuego 10-16, Tierra 8-13, Aire 7-11 con bono a distancia, Agua 7-11). Aire puede esquivar (25% +5% si lejos). Curar: 65-70% vida perdida, Agua 75-90%.
- **Orcos**: hacha 1-5 solo cerca, aplica sangrado (3 por 2 turnos). Martillo 2-7 solo cerca. Curar: 25-45% ahora + cura diferida 5-25% siguiente turno.
- **Bestias**: punos 20-30 y se autodanian 10; espada 1-10. Curar: 50% vida perdida.
- **Turnos**: alternos; efectos de sangrado/curacion diferida se aplican al inicio del turno del combatiente.
- **Victoria**: cuando un combatiente cae a 0 o menos se registra ganador, turnos y configuraciones, se guardan estadisticas y se navega a `MatchSummaryPage`.
