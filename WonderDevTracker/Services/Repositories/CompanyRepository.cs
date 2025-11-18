using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                   IServiceScopeFactory scopeFactory) : ICompanyRepository
    {
        public async Task<Company> GetCompanyAsync(UserInfo userInfo)
        {
            await using var context = contextFactory.CreateDbContext();
            Company company = await context.Companies
                .Include(c => c.Members)
                .Include(c => c.Invites)
                .FirstAsync(c => c.Id == userInfo.CompanyId); //Cannot be null
            return company;
        }
        public async Task UpdateCompanyAsync(Company company, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin) || company.Id != userInfo.CompanyId) return;
            
            await using var context = contextFactory.CreateDbContext();
            FileUpload? existingImage = null;
            //check if new image is being added
            if (company.Image is not null && company.Image.Id != company.ImageId)
            {
                //fetch existing image to delete after save changes
                existingImage = await context.Companies
                    .Where(c => c.Id == userInfo.CompanyId)
                    .Select(c => c.Image)
                    .FirstOrDefaultAsync();

                context.Add(company.Image);//save new image
                company.ImageId = company.Image.Id;//update foreign key
            }
            context.Update(company);
            await context.SaveChangesAsync();//save new image and company changes first
            
            if (existingImage is not null)
            {
                context.Remove(existingImage);//remove old image
                await context.SaveChangesAsync();//save changes again
            }

            
        }

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
            await using var scope = scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var usersInRole = await userManager.GetUsersInRoleAsync(role.ToString());
            return [.. usersInRole.Where(u => u.CompanyId == userInfo.CompanyId)];
        }

        
       
        #endregion
    }
}
