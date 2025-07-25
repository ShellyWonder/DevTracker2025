﻿using System.ComponentModel.DataAnnotations;
using static MudBlazor.CategoryTypes;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class TicketAttachmentDTO
    {
        private DateTimeOffset _created;

        public int Id { get; set; }

        [Required]
        public string? FileName { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        //navigation properties --all strings are nullable
        public required string AttachmentUrl { get; set; }
        public int TicketId { get; set; }
        public string? UserId { get; set; }
        public AppUserDTO? User { get; set; }
    }
}
