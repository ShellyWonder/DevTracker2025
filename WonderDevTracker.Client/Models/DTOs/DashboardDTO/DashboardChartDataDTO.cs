namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class DashboardChartDataDTO
    {
        public DashboardTicketsOverTimeChartDTO TicketsOverTimeChart { get; set; } = new DashboardTicketsOverTimeChartDTO();
        public List<DashboardCountByCategoryDTO> TicketsByStatus { get; set; } = [];
        public List<DashboardCountByCategoryDTO> TicketsByPriority { get; set; } = [];
        public List<DashboardCountByCategoryDTO> ProjectsByPriority { get; set; } = [];
        public List<DashboardTicketsByProjectDTO> TicketsByProject { get; set; } = [];
    }
}
