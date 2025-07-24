//ICompanyRepository


//ICompanyRepository

using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ICompanyRepository
    {

        public Task<Company?> GetCompanyInfoByIdAsync(int companyId);
        public Task<List<ApplicationUser>>GetAllMembersAsync(int companyId);
        public Task<List<Project>>GetAllProjectsAsync(int companyId);
        public Task<List<Ticket>>GetAllTicketsAsync(int companyId);
        public Task<List<Invite>>GetAllInvitesAsync(int companyId);

        
    }

}

