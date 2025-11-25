using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class InviteRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IInviteRepository
    {
        public async Task<Invite> CreateInviteAsync(Invite invite, UserInfo user)
        {
            await using ApplicationDbContext db = contextFactory.CreateDbContext();

            await EnsureInviteConditionsAreValidAsync(invite, user, db);

            invite.CompanyToken = Guid.NewGuid();
            invite.CompanyId = user.CompanyId;
            invite.InviteDate = DateTimeOffset.UtcNow;
            invite.InvitorId = user.UserId;
            invite.IsValid = true;
            invite.InviteeId = null; // Invitee not yet registered ~ Id set upon registration
            invite.JoinDate = null; // Invitee not yet registered ~ JoinDate set upon registration

            db.Invites.Add(invite);
            await db.SaveChangesAsync();
            return invite;

        }

        public async Task<IEnumerable<Invite>> GetInviteAsync(UserInfo user)
        {
            await using ApplicationDbContext db = contextFactory.CreateDbContext();
            List<Invite> invites = await db.Invites
                 .Where(i => i.CompanyId == user.CompanyId)
                 .Include(i => i.Invitor)
                 .Include(i => i.Invitee)
                 .Include(i => i.Project)
                 .ToListAsync();
            foreach (Invite invite in invites)
            {
                invite.IsValid = ValidateInvite(invite);
            }
            await db.SaveChangesAsync();
            return invites;
        }

        public async Task CancelInviteAsync(int inviteId, UserInfo user)
        {
            if (!user.IsInRole(Role.Admin))
                throw new ApplicationException($"{user.Email} is not authorized to cancel invites.");
            await using ApplicationDbContext db = contextFactory.CreateDbContext();
            Invite? invite = await db.Invites
                .FirstOrDefaultAsync(i => i.Id == inviteId && i.CompanyId == user.CompanyId
                                                                  && i.IsValid == true);
            if (invite != null)
            {
                invite.IsValid = false;
                await db.SaveChangesAsync();
            }
        }
        #region PRIVATE HELPER METHODS
        private static async Task<bool> EnsureInviteConditionsAreValidAsync(Invite invite, UserInfo user, ApplicationDbContext db)
        {
            // 1. Only Admins may create invites
            if (!user.IsInRole(Role.Admin))
                throw new ApplicationException($"{user.Email} is not authorized to create invites.");


            // 2. Ensure invite is sent only to NEW users
            bool emailExists = await db.Users.AnyAsync(u =>
                u.Email == invite.InviteeEmail &&
                u.CompanyId == user.CompanyId);

            if (emailExists)
                throw new ApplicationException(
                    $"User with email {invite.InviteeEmail} already exists within the company.");

            // 3. Ensure project belongs to same company
            bool projectExists = await db.Projects.AnyAsync(p =>
                p.Id == invite.ProjectId &&
                p.CompanyId == user.CompanyId);

            if (!projectExists)
                throw new ApplicationException(
                    $"Project with ID {invite.ProjectId} does not exist within the company.");

            // If all conditions passed, return true
            return true;
        }

        private bool ValidateInvite(Invite invite)
        {
            bool isValid = invite.IsValid
                && DateTimeOffset.UtcNow < invite.InviteDate.AddDays(7)
                && invite.JoinDate is null
                && string.IsNullOrEmpty(invite.InviteeId);
            return isValid;
        }


        #endregion

    }
}
