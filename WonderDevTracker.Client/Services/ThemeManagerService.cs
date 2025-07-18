using Blazored.LocalStorage;
using MudBlazor;
using WonderDevTracker.Client.Themes;

namespace WonderDevTracker.Client.Services
{
    public class ThemeManagerService(ILocalStorageService localStorage)
    {
        private readonly ILocalStorageService _localStorage = localStorage;
        private const string _themePreferenceKey = "user-theme";

        public MudTheme CurrentTheme { get; private set; } = AppThemes.PaletteLightTheme;
        public bool IsDarkMode => CurrentTheme == AppThemes.PaletteDarkTheme;

        public event Action? OnThemeChanged;

        public async Task InitializeAsync()
        {
            try
            {
                var savedTheme = await _localStorage.GetItemAsync<string>(_themePreferenceKey);
                CurrentTheme = savedTheme == "dark" ? AppThemes.PaletteDarkTheme : AppThemes.PaletteLightTheme;
            }
            catch
            {
                // fallback to default
                CurrentTheme = AppThemes.PaletteLightTheme;
            }
            NotifyThemeChanged();
        }


        public async Task ToggleThemeAsync()
        {
            if (IsDarkMode)
            {
                CurrentTheme = AppThemes.PaletteLightTheme;
                await _localStorage.SetItemAsync(_themePreferenceKey, "light");
            }
            else
            {
                CurrentTheme = AppThemes.PaletteDarkTheme;
                await _localStorage.SetItemAsync(_themePreferenceKey, "dark");
            }

            NotifyThemeChanged();
        }

        private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
    }
}