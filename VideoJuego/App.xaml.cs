using System;
using System.Linq;
using VideoJuego.Resources.Themes;

namespace VideoJuego
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var savedTheme = Microsoft.Maui.Storage.Preferences.Get("theme", "dark");
            UserAppTheme = savedTheme == "light" ? AppTheme.Light : AppTheme.Dark;
            ApplyThemeDictionary(savedTheme);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        public void ApplyThemeDictionary(string theme)
        {
            var merged = Resources.MergedDictionaries;
            var themeDictionaries = merged.Where(d =>
                d is LightTheme or DarkTheme ||
                (d.Source?.OriginalString?.Contains("Resources/Themes", StringComparison.OrdinalIgnoreCase) == true)).ToList();
            foreach (var dict in themeDictionaries)
            {
                merged.Remove(dict);
            }

            var newDict = theme == "light"
                ? (ResourceDictionary)new LightTheme()
                : new DarkTheme();

            merged.Add(newDict);
        }
    }
}
