using Microsoft.Extensions.Logging;
using VideoJuego.Services;
using VideoJuego.ViewModels;
using VideoJuego.Views;

namespace VideoJuego;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();
        builder.Services.AddSingleton<IMatchRepository, MatchRepository>();
        builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IImageService, ImageService>();
        builder.Services.AddSingleton<ICombatService, CombatService>();
        builder.Services.AddSingleton<ISaveGameService, SaveGameService>();

        builder.Services.AddSingleton<MainMenuViewModel>();
        builder.Services.AddSingleton<PlayerSetupViewModel>();
        builder.Services.AddSingleton<CharacterSelectionViewModel>();
        builder.Services.AddSingleton<BattleViewModel>();
        builder.Services.AddSingleton<MatchSummaryViewModel>();
        builder.Services.AddSingleton<StatsViewModel>();
        builder.Services.AddSingleton<LoadGameViewModel>();

        builder.Services.AddSingleton<MainMenuPage>();
        builder.Services.AddSingleton<PlayerSetupPage>();
        builder.Services.AddSingleton<CharacterSelectionPage>();
        builder.Services.AddSingleton<BattlePage>();
        builder.Services.AddSingleton<MatchSummaryPage>();
        builder.Services.AddSingleton<StatsPage>();
        builder.Services.AddSingleton<CreditsPage>();
        builder.Services.AddSingleton<GuidePage>();
        builder.Services.AddSingleton<LoadGamePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
            db.InitializeAsync().GetAwaiter().GetResult();
        }

        return app;
    }
}