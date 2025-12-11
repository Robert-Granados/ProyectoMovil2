namespace VideoJuego
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("PlayerSetupPage", typeof(Views.PlayerSetupPage));
            Routing.RegisterRoute("CharacterSelectionPage", typeof(Views.CharacterSelectionPage));
            Routing.RegisterRoute("BattlePage", typeof(Views.BattlePage));
            Routing.RegisterRoute("MatchSummaryPage", typeof(Views.MatchSummaryPage));
            Routing.RegisterRoute("StatsPage", typeof(Views.StatsPage));
            Routing.RegisterRoute("CreditsPage", typeof(Views.CreditsPage));
            Routing.RegisterRoute("GuidePage", typeof(Views.GuidePage));
            Routing.RegisterRoute("LoadGamePage", typeof(Views.LoadGamePage));
        }
    }
}
