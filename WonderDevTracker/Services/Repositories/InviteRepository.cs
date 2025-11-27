using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Data;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IEmailSender _emailSender;
        private readonly IDataProtector _protector;

        public InviteRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                IEmailSender emailSender,
                                IDataProtectionProvider dataProtectionProvider,
                                IConfiguration config)
        {
            _contextFactory = contextFactory;
            _emailSender = emailSender;
            _protector = dataProtectionProvider.CreateProtector("InviteProtectionKey"); 

            string protectionPurpose = config["InviteProtectionKey"]
                ?? throw new ApplicationException("InviteProtectionKey not found in configuration");
            
        }

        public async Task<Invite> CreateInviteAsync(Invite invite, UserInfo user)
        {
            await using ApplicationDbContext db = _contextFactory.CreateDbContext();

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

        public async Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo user)
        {
            //validate user is admin
            if (!user.IsInRole(Role.Admin)) return false;

            try
            {
                await using ApplicationDbContext context = _contextFactory.CreateDbContext();
                //validate invite belongs to company
                Invite? invite = await context.Invites
                                       .Include(i => i.Company)
                                       .Include(i => i.Invitor)
                                       .Include(i => i.Invitee)
                                       .Include(i => i.Project)
                                       .FirstOrDefaultAsync(i => i.Id == inviteId
                                       && i.CompanyId == user.CompanyId
                                       && i.IsValid);
                //validate invite exists and is valid
                if (invite is null || ValidateInvite(invite) == false)
                {
                    if (invite is not null)
                    {
                        invite.IsValid = false;
                        await context.SaveChangesAsync();
                    }

                    return false;
                }

                //generate invite link:
                //encrypt company token
                string protectedToken = _protector.Protect(invite.CompanyToken.ToString());
                //encrypt invite's company id
                string protectedCompanyId = _protector.Protect(invite.CompanyId.ToString());
                //encrypt invitee email 
                string protectedEmail = _protector.Protect(invite.InviteeEmail!);
                string baseUrl = baseUri.GetLeftPart(UriPartial.Authority);
                string inviteUrl = $"{baseUrl}/Account/Register/Invite?token={protectedToken}&email={protectedEmail}&company={protectedCompanyId}";

                var inviteeFirstName = invite.Invitee?.FirstName ?? invite.InviteeFirstName ?? "friend";
                var inviteeLastName = invite.Invitee?.LastName ?? invite.InviteeLastName ?? string.Empty;
                //Construct the email
                string subject = $"Please join {invite.Company!.Name} on DevTracker.";
                string message = string.IsNullOrEmpty(invite.Message)
                                 ? string.Empty
                                 : $""" 
                                    <hr />
                                    <p>Message from {invite.Invitor!.FirstName} {invite.Invitor!.LastName}:</p>
                                    <blockquote>{invite.Message}</blockquote>
                                    <hr />
                                    """; 
                 string body = $""" 
                                <h1>Welcome to Dev Tracker, {inviteeFirstName} {inviteeLastName}!</h1>
                                <p> {invite.Invitor!.FirstName} {invite.Invitor!.LastName} has invited you to join the
                                {invite.Company!.Name} team to work on the company's {invite.Project!.Name} project.
                                </p>
                                <p>
                                {message}
                                </p>
                                <p>
                                To accept this invitation, please <a href="{inviteUrl}">click here</a> to register a new account.
                                </p>
                                <p>
                                <strong>NOTE:</strong> This invitation expires on {invite.InviteDate.AddDays(7):d}
                                </p>
                                <small>
                                <p>If you are unable to click the link above, please copy and paste the following URL into your browser's address bar.</p>
                                <br>
                                {inviteUrl}
                                </small>
                                """;

                await _emailSender.SendEmailAsync(invite.InviteeEmail!,subject, body);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }


        public async Task<IEnumerable<Invite>> GetInviteAsync(UserInfo user)
        {
            await using ApplicationDbContext db = _contextFactory.CreateDbContext();
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
            await using ApplicationDbContext db = _contextFactory.CreateDbContext();
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

        private static bool ValidateInvite(Invite invite)
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
