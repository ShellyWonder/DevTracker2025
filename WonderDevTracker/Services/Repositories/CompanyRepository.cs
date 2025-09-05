using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                   UserManager<ApplicationUser> userManager ) : ICompanyRepository
    {
        #region GET ALL USERS BY COMPANY ID
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();

            List<ApplicationUser> users = await context.Users
                .Where(u => u.CompanyId == userInfo.CompanyId)
                .ToListAsync();

            return users;
        }
        #endregion

        #region GET USERS IN ROLE 
        public async Task<IReadOnlyList<ApplicationUser>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();
            var usersInRole = await userManager.GetUsersInRoleAsync(role.ToString());
            return [.. usersInRole.Where(u => u.CompanyId == userInfo.CompanyId)];
        }

        #endregion
    }
}
