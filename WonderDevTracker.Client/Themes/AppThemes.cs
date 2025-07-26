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
            },

            Typography = GetDefaultTypography()
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
            },
            
            Typography = GetDefaultTypography()
        };

        private static Typography GetDefaultTypography()
        {
            return new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = ["Inter", "sans-serif"],
                    FontSize = "1rem",
                    LineHeight = "1.5"
                },
                H1 = new H1Typography { FontSize = "2.25rem", FontWeight = "600", FontFamily = ["Poppins", "sans-serif"] },
                H2 = new H2Typography { FontSize = "2rem", FontWeight = "500", FontFamily = ["Poppins", "sans-serif"] },
                H3 = new H3Typography { FontSize = "1.75rem", FontWeight = "500", FontFamily = ["Poppins", "sans-serif"] },
                H4 = new H4Typography { FontSize = "1.5rem", FontWeight = "500", FontFamily = ["Poppins", "sans-serif"] },
                H5 = new H5Typography { FontSize = "1.25rem", FontWeight = "500", FontFamily = ["Poppins", "sans-serif"] },
                H6 = new H6Typography { FontSize = "1.1rem", FontWeight = "500", FontFamily = ["Poppins", "sans-serif"] },
                Subtitle1 = new Subtitle1Typography { FontSize = "1rem", FontWeight = "500", FontFamily = ["Inter", "sans-serif"] },
                Subtitle2 = new Subtitle2Typography { FontSize = "0.875rem", FontWeight = "500", FontFamily = ["Inter", "sans-serif"] },
                Body1 = new Body1Typography { FontSize = "1rem", FontWeight = "400", FontFamily = ["Inter", "sans-serif"] },
                Body2 = new Body2Typography { FontSize = "0.9rem", FontWeight = "400", FontFamily = ["Inter", "sans-serif"] },
                Button = new ButtonTypography { FontSize = "0.95rem", FontWeight = "500", FontFamily = ["Inter", "sans-serif"] },
                Caption = new CaptionTypography
                {
                    FontSize = "0.8rem",
                    FontWeight = "400",
                    FontFamily = ["JetBrains Mono", "monospace"]
                },
                Overline = new OverlineTypography
                {
                    FontSize = "0.75rem",
                    FontWeight = "400",
                    FontFamily = ["JetBrains Mono", "monospace"],
                    TextTransform = "uppercase"
                }
            };
        }
    }
}