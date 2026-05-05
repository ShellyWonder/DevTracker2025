namespace WonderDevTracker.Client.Models.DTOs.DashboardDTO
{
    public class CompanyDashboardInfoDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public string CompanyDescription { get; set;} = string.Empty;

        //public string? Image { get; set; }
    }
}
