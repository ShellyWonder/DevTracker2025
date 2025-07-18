using Blazored.LocalStorage;
using MudBlazor;
using WonderDevTracker.Client.Themes;

namespace WonderDevTracker.Client.Services
{
    public class ThemeManagerService(ILocalStorageService localStorage)
    {
        private readonly ILocalStorageService _localStorage = localStorage;
        private const string ThemePreferenceKey = "user-theme";

        public MudTheme CurrentTheme { get; private set; } = AppThemes.Light;
        public bool IsDarkMode => CurrentTheme == AppThemes.Dark;

        public event Action? OnThemeChanged;

        public async Task InitializeAsync()
        {
            try
            {
                var savedTheme = await _localStorage.GetItemAsync<string>(ThemePreferenceKey);
                CurrentTheme = savedTheme == "dark" ? AppThemes.Dark : AppThemes.Light;
            }
            catch
            {
                // fallback to default
                CurrentTheme = AppThemes.Light;
            }
            NotifyThemeChanged();
        }


        public async Task ToggleThemeAsync()
        {
            if (IsDarkMode)
            {
                CurrentTheme = AppThemes.Light;
                await _localStorage.SetItemAsync(ThemePreferenceKey, "light");
            }
            else
            {
                CurrentTheme = AppThemes.Dark;
                await _localStorage.SetItemAsync(ThemePreferenceKey, "dark");
            }

            NotifyThemeChanged();
        }

        private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
    }
}