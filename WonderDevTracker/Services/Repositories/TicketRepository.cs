using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class TicketRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                   UserManager<ApplicationUser>userManager) : ITicketRepository
    {
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
