using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Data;

namespace WonderDevTracker.Services.Repositories
{
    //Company info methods, shared across all dashboard types
    public partial class  DashboardRepository
    {
       
        private static async Task<CompanyDashboardInfoDTO> GetCompanyInfoAsync(ApplicationDbContext context, int companyId)
        {
            return await context.Companies
                .Where(c => c.Id == companyId)
                .Select(c => new CompanyDashboardInfoDTO
                {
                    CompanyId = c.Id,
                    CompanyName = c.Name ?? "Company",
                    //ImageUrl = c.Image != null ? $"/fileuploads/{c.Image.Id}" : null
                })
                .FirstOrDefaultAsync() 
                              ?? throw new ApplicationException("Company not found");
        }
      

    }
}
