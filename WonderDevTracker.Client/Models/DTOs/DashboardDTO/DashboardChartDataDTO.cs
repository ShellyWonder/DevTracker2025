namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DashboardChartDataDTO
    {
        public List<DashboardMonthlyTicketsDTO> TicketsOverTime { get; set; } = [];
        public List<DashboardMonthlyTicketsDTO> ResolvedTicketsOverTime { get; set; } = [];
    }
}
