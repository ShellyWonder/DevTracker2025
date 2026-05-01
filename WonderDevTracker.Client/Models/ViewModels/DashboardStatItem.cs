using MudBlazor;

namespace WonderDevTracker.Client.Models.ViewModels
{
    public class DashboardStatItem
    {
        public string? Title { get; set; }
        public int Value { get; init; }

        public string Icon { get; init; } = Icons.Material.Filled.QueryStats;

        public Color Color { get; init; } = Color.Primary;
    }
}
