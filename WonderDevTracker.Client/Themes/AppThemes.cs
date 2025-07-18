using MudBlazor;


namespace WonderDevTracker.Client.Themes
{
    public static class AppThemes
    {

        public static readonly MudTheme PaletteLightTheme = new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#0077B6",
                Secondary = "#00B4D8",
                Success = "#90BE6D",
                Info = "#6895ED", //periwinkle

                Warning = "#FF993b",   // orange
                Error = "#E63946",     // reddish tone
                Dark = "#1E1E2F",      // deep purple/black (for dark buttons)

                Background = "F9FCFE",
                Surface = "#FFFFFF",
                AppbarBackground = "#0077B6",
                DrawerBackground = "rgb(240, 240,240)",

                TextPrimary = "#1E1E2F",
                TextSecondary = "#6C7A89",
                LinesDefault = "#DDE6EC"

            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "6px"
            }
        };
        public static readonly MudTheme PaletteDarkTheme = new()
        {
            PaletteDark = new PaletteDark
            {
                Primary = "#0077B6",
                Secondary = "#00B4D8",
                Success = "#90BE6D",
                Info = "#5D81C9",

                Warning = "#FF993b", // e.g. bright orange
                Error = "#E63946",   // red tone
                Dark = "#343A40",     // for dark button styling

                Background = "#1E1E2F",
                Surface = "#2C3E50",
                AppbarBackground = "#2C3E50",
                DrawerBackground = "#1E1E2F",

                TextPrimary = "#F8F9FA",
                TextSecondary = "#A8B2C1",
                LinesDefault = "#2E3A59"
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "6px"
            }
        };
    }
}