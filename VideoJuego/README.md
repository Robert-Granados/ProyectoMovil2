# Guerra de Reinos - .NET MAUI

Juego de combate por turnos, pass-and-play para Android. Dos jugadores eligen raza y variante, se mueven por distancia (1-3), 
atacan o se curan segun sus reglas y gana quien deja al rival en 0 de vida. Tema con acentos morados y soporte de modo claro/oscuro.

# Integrantes del proyecto:
- Tiffany Alfaro Valverde - 
- Robert Granodos Perez   -
- Seidy OPorta Rodriguuez -

# Como Instalar el APK
1. Descargar el APK 
2. Habilitar la instalacion de aplicaciones de fuentes desconocidas en el dispositivo Android.
3. Abrir el APK descargado en el dispositivo.

# Requisitos para ejecutar el juego
- Dispositivo Android con version que soporte la aplicacion (Android 10 o superior recomendado).
- Habilitar la instalacion de aplicaciones de fuentes desconocidas en el dispositivo.

# Instrucciones para jugar
1. Abrir la aplicacion "Guerra de Reinos" en el dispositivo Android.
2. Seleccionar "Nueva partida" o "Continuar" si hay un guardado.
3. Ingresar los nombres de los jugadores.
4. Seleccionar las razas y variantes de cada jugador.
5. Iniciar el combate.
6. Jugar por turnos, eligiendo atacar, curar o moverse.
7. Al finalizar el combate, se mostrara un resumen con el ganador y las estadisticas de la partida.

# Tecnologias usadas 
- Visual Studio 2022 con .NET MAUI.
- C# como lenguaje de programacion.
- SQLite para persistencia de datos.
- XAML para la interfaz de usuario.
- CommunityToolkit.Mvvm para el manejo de ViewModels.

 
# Descripcion de la arquitectura
- Models: configuraciones de personaje (`CharacterConfig`), estados de batalla (`CombatState`, `Combatant`, `CombatActionType`), perfiles (`PlayerProfile`, `MatchRecord`, `BattleResult`, enums de razas/armas).
- Services: `CombatService` (reglas de batalla, distancia, dano, curas, sangrado, esquivas), `ImageService` (imagen por raza), `SessionService` (estado en memoria), repositorios SQLite (`DatabaseService`, 
`PlayerRepository`, `CharacterRepository`, `MatchRepository`, `SaveGameService`).
- ViewModels: `MainMenuViewModel`, `PlayerSetupViewModel`, `CharacterSelectionViewModel`, `BattleViewModel`, `MatchSummaryViewModel`, `StatsViewModel` usan `CommunityToolkit.Mvvm`.
- Views: paginas XAML vinculadas a cada VM; navegacion via `Shell`.

# Logica de batalla (resumen)
- Distancia: entero 0..2 (0=cuerpo a cuerpo, 2=lejano). Avanzar reduce, Retroceder aumenta. Solo rifle francotirador y hechizo de Aire funcionan cuando distancia > 0; rifle en 2 hace 10-20 de dano, Aire a distancia suma 5-15%.
- Vida inicial: 100; elfo de Agua inicia con 115.
- Humanos: escopeta dano 1-5 + bonificacion 0-2 (solo distancia 0); rifle 1-5 en melee, 10-20 en distancia 2. Curar 41-49% de vida perdida.
- Elfos: magia por elemento (Fuego 10-16, Tierra 8-13, Aire 7-11 con bono a distancia, Agua 7-11). Aire puede esquivar (25% +5% si lejos). Curar: 65-70% vida perdida, Agua 75-90%.
- Orcos: hacha 1-5 solo cerca, aplica sangrado (3 por 2 turnos). Martillo 2-7 solo cerca. Curar: 25-45% ahora + cura diferida 5-25% siguiente turno.
- Bestias: punos 20-30 y se autodanian 10; espada 1-10. Curar: 50% vida perdida.
- Turnos: alternos; efectos de sangrado/curacion diferida se aplican al inicio del turno del combatiente.
- Victoria: cuando un combatiente cae a 0 o menos se registra ganador, turnos y configuraciones, se guardan estadisticas y se navega a `MatchSummaryPage`.

# Persistencia
- SQLite en `AppDataDirectory/fantasy_duel.db3`.
- Tablas: `Players` (wins/losses/draws), `Characters` (ultima configuracion), `Matches` (p1/p2, razas, variantes, ganador, empate, fecha), `SavedGames` (estado de combate para continuar).

# Pantallas
- MainMenuPage: titulo "Guerra de Reinos", botones Nueva partida, continuar (si hay guardado), Estadisticas, Creditos y Guia. Incluye switch de modo nocturno.
- PlayerSetupPage: nombres de jugadores.
- CharacterSelectionPage: pickers por raza con controles dependientes (arma/elemento/ataque) y boton "Comenzar combate" con validaciones.
- BattlePage: muestra vida de ambos, distancia, turno actual, imagen segun raza, botones Avanzar/Retroceder/Curar, ataque contextual, guardar y reiniciar partida, log tipo consola.
- MatchSummaryPage: ganador, vida restante, razas/variantes y turnos; botones a menu o estadisticas.
- StatsPage: lista de jugadores con victorias/derrotas/empates y boton Recargar.
- CreditsPage y GuidePage: contenido informativo.

LAS IMAGNES ESTAN EN LA CARPETA CAPTURA DE PANTALLA PRINCIPALES

# Persistencia y ejemplos de uso
- Al finalizar una partida, `BattleViewModel` llama a `PlayerRepository.UpdateStatsAsync`, guarda configuraciones en `CharacterRepository` y registra el match en `MatchRepository` con nombres, razas y variantes.
- `StatsViewModel.LoadAsync` lee jugadores ordenados por victorias para mostrarlos en `StatsPage`.

# Ejecucion
Abrir `VideoJuego.sln` en Visual Studio 2022 (workload MAUI), seleccionar Android y ejecutar (F5) en emulador o dispositivo. El tema se puede alternar desde el menu principal.

# Diagrama Explicativo
┌─────────────────────────────────────────────────────────────┐
│                      VIDEOJUEGO (.NET MAUI)                 │
├─────────────────────────────────────────────────────────────┤
│                         ARQUITECTURA                        │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                           PRESENTACIÓN                      │
├─────────────────────────────────────────────────────────────┤
│    AppShell ──┐        │    MainPage          │    GlobalXmlns  │
│    (Navegación)        │    (Página por defecto)   │ (Espacios XML) │
├─────────────────────────────────────────────────────────────┤
│                        VIEWS (XAML Pages)                   │
├─────────────────────────────────────────────────────────────┤
│  MainMenuPage │ PlayerSetupPage │ CharacterSelectionPage    │
│  BattlePage   │ MatchSummaryPage│ StatsPage │ CreditsPage  │
│  GuidePage    │ LoadGamePage                               │
├─────────────────────────────────────────────────────────────┤
│                      VIEWMODELS (MVVM)                      │
├─────────────────────────────────────────────────────────────┤
│  MainMenuViewModel      │ PlayerSetupViewModel             │
│  CharacterSelectionVM   │ BattleViewModel                  │
│  MatchSummaryViewModel  │ StatsViewModel                   │
│  LoadGameViewModel      │ (CommunityToolkit.Mvvm)         │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                          SERVICIOS                          │
├─────────────────────────────────────────────────────────────┤
│  CombatService    │ SessionService     │ ImageService       │
│  (Lógica de batalla, distancia, daño, curas, efectos)      │
├─────────────────────────────────────────────────────────────┤
│                    REPOSITORIOS (SQLite)                    │
├─────────────────────────────────────────────────────────────┤
│  DatabaseService  │ PlayerRepository   │ MatchRepository    │
│  CharacterRepository │ SaveGameService                    │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                           MODELOS                           │
├─────────────────────────────────────────────────────────────┤
│  CharacterConfig │ CombatState │ Combatant │ PlayerProfile │
│  MatchRecord     │ BattleResult│ Enums (Raza, Arma, etc.) │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                         INFRAESTRUCTURA                     │
├─────────────────────────────────────────────────────────────┤
│  MauiProgram.cs   │ App.xaml/.cs   │ AppShell.xaml/.cs     │
│  (Config DI, Registro de Servicios, Rutas, Tema Claro/Oscuro)│
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                         FLUJO PRINCIPAL                     │
├─────────────────────────────────────────────────────────────┤
│  1. MauiProgram → Registra servicios y ViewModels           │
│  2. AppShell → Define rutas de navegación                   │
│  3. App → Aplica tema (claro/oscuro) desde preferencias     │
│  4. MainMenuPage → Nueva partida/Continuar/Estadísticas     │
│  5. PlayerSetup → Nombres → CharacterSelection → Configura  │
│  6. BattlePage → Combate por turnos con servicios de batalla│
│  7. MatchSummary → Resumen → StatsPage o MainMenu           │
│  8. Persistencia: SQLite guarda jugadores, partidas, guardados│
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                     DEPENDENCIAS EXTERNAS                   │
├─────────────────────────────────────────────────────────────┤
│  .NET MAUI        │ CommunityToolkit.Mvvm │ Microsoft.Data.Sqlite│
│  SQLitePCLRaw     │ XAML                  │ C#               │
└─────────────────────────────────────────────────────────────┘
