using MudBlazor;

namespace WonderDevTracker.Client.Models.ViewModels
{
    public class DashboardStatItemViewModel
    {
        public string Title { get; set; } = string.Empty;

        public int Value { get; init; }

        public string Icon { get; init; } = Icons.Material.Filled.QueryStats;

        public Color Color { get; init; } = Color.Primary;
    }
}
