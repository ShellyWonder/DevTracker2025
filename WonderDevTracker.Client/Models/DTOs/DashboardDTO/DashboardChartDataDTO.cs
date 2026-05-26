using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    
    public class DashboardChartDataDTO
    {
        public DashboardTicketsOverTimeChartDTO TicketsOverTimeChart { get; set; } = new DashboardTicketsOverTimeChartDTO();
        public List<DashboardEnumCountDTO<TicketStatus>> TicketsByStatus { get; set; } = [];
        public List<DashboardEnumCountDTO<TicketPriority>> TicketsByPriority { get; set; } = [];
        public List<DashboardEnumCountDTO<ProjectPriority>> ProjectsByPriority { get; set; } = [];
        public List<DashboardTicketsByProjectDTO> TicketsByProject { get; set; } = [];
    }
}
