using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class InviteDTO
    {
        //backing field for InviteDate to ensure it is stored in UTC
        private DateTimeOffset _inviteDate;
        private DateTimeOffset? _joinDate;

        public int Id { get; set; }

        public DateTimeOffset InviteDate
        {
            get => _inviteDate;
            set => _inviteDate = value.ToUniversalTime();
        }

        public DateTimeOffset? JoinDate
        {
            get => _joinDate;
            set => _joinDate = value?.ToUniversalTime();
        }
        public Guid CompanyToken { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string? InviteeEmail { get; set; }

        [Required]
        public string? InviteeFirstName { get; set; }
        public string? InviteeLastName { get; set; }

        public string? Message { get; set; }

        public bool IsValid { get; set; } = false;

        //navigation properties
        public int CompanyId { get; set; }
        public CompanyDTO? Company { get; set; }

        public ProjectDTO? Project { get; set; }
        public int ProjectId { get; set; }


        [Required]
        public string? InvitorId { get; set; }
        public  AppUserDTO? Invitor { get; set; }

        public string? InviteeId { get; set; }
        public  AppUserDTO? Invitee { get; set; }


    }
}
