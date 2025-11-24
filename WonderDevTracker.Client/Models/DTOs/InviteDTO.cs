using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [Display(Name = "Join Date")]
        public DateTimeOffset? JoinDate
        {
            get => _joinDate;
            set => _joinDate = value?.ToUniversalTime();
        }
       

        [Required(ErrorMessage ="Invitee Email Address is required")]
        [EmailAddress]
        [Description("Invitee email address")]
        [Display(Name = "Invitee Email")]
        public string? InviteeEmail { get; set; }

        [Required(ErrorMessage = "Invitee First Name is required")]
        [Description("Invitee first name")]
        [Display(Name = "Invitee First Name")]
        public string? InviteeFirstName { get; set; }

        [Required(ErrorMessage = "Invitee Last Name is required")]
        [Description("Invitee last name")]
        [Display(Name = "Invitee Last Name")]
        public string? InviteeLastName { get; set; }

        [Description("Invitee full name")]
        [Display(Name = "Invitee Full Name")]
        [NotMapped, JsonIgnore]
        public string? InviteeFullName => $"{InviteeFirstName} {InviteeLastName}";
        


        [Description("Optional message in email body")]
        public string? Message { get; set; }

        [Description("Indicates whether the invite is currently valid - invites are valid for 7 days.")]
        public bool IsValid { get; set; } = false;

        //navigation properties
        [Description("Company Id of the Admin sending the invite.")]
        public int CompanyId { get; set; }
        [Description("The project on whose behalf the Company Administration is sending the invite")]
        public ProjectDTO? Project { get; set; }

        [Required(ErrorMessage = "Project selection is required")]
        [Description("The  Id of the project on whose behalf the Company Administration is sending the invite.")]
        public int? ProjectId { get; set; } //the id is null until selection is made in the UI

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
