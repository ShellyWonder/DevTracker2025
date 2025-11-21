using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class InviteDTO
    {
        //backing field for InviteDate to ensure it is stored in UTC
        private DateTimeOffset _inviteDate;
        private DateTimeOffset? _joinDate;

        [Description("Unique identifier for the invite")]
        public int Id { get; set; }

        [Description("Date the invite was sent in UTC")]
        public DateTimeOffset InviteDate
        {
            get => _inviteDate;
            set => _inviteDate = value.ToUniversalTime();
        }

        [Description("Date the invite was accepted in UTC")]
        public DateTimeOffset? JoinDate
        {
            get => _joinDate;
            set => _joinDate = value?.ToUniversalTime();
        }
       

        [Required, EmailAddress]
        [Description("Invitee email address")]
        public string? InviteeEmail { get; set; }

        [Required]
        [Description("Invitee first name")]
        public string? InviteeFirstName { get; set; }

        [Required]
        [Description("Invitee last name")]
        public string? InviteeLastName { get; set; }

        [Required]
        [Description("Optional message in email body")]
        public string? Message { get; set; }

        [Description("Indicates whether the invite is currently valid - invites are valid for 7 days.")]
        public bool IsValid { get; set; } = false;

        //navigation properties
        [Description("The project on whose behalf the Company Administration is sending the invite")]
        public ProjectDTO? Project { get; set; }

        [Description("The  Id of the project on whose behalf the Company Administration is sending the invite")]
        public int ProjectId { get; set; }

        [Required]
        [Description("Id of the Company Administration sending the invite")]
        public string? InvitorId { get; set; }

        [Description("Id of the Company Administration sending the invite")]
        public  AppUserDTO? Invitor { get; set; }

        [Description("Id of the user who accepted the invite")]
        public string? InviteeId { get; set; }

        [Description("The user who accepted the invite")]
        public  AppUserDTO? Invitee { get; set; }


    }
}
