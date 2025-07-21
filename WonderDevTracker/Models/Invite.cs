﻿using System.ComponentModel.DataAnnotations;
using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Models
{
    public class Invite
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
        public virtual Company? Company { get; set; }

        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string? InvitorId { get; set; }
        public virtual ApplicationUser? Invitor { get; set; }
       
        public string? InviteeId { get; set; }
        public virtual ApplicationUser? Invitee { get; set; }

    }
    public static class InviteExtensions
    {
        public static InviteDTO ToDTO(this Invite invite)
        {
            InviteDTO dto = new()
            {
                Id = invite.Id,
                InviteDate = invite.InviteDate,
                JoinDate = invite.JoinDate,
                CompanyToken = invite.CompanyToken,
                InviteeEmail = invite.InviteeEmail,
                InviteeFirstName = invite.InviteeFirstName,
                InviteeLastName = invite.InviteeLastName,
                Message = invite.Message,
                IsValid = invite.IsValid,
                CompanyId = invite.CompanyId,
                Company = invite.Company?.ToDTO(),
                ProjectId = invite.ProjectId,
                Project = invite.Project?.ToDTO(),
                InvitorId = invite.InvitorId,
                InviteeId = invite.InviteeId,
                Invitor = invite.Invitor?.ToDTO(),
                Invitee = invite.Invitee?.ToDTO()

            };
            
            return dto;
        }
           
    }
    
}