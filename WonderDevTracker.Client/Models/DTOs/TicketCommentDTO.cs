﻿using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class TicketCommentDTO
    {

        private DateTimeOffset _created;

        public int Id { get; set; }
        [Required]
        public string? Content { get; set; } = string.Empty;

        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public bool Edited { get; set; } = false;

        //navigation properties
        public int TicketId { get; set; }
      
        [Required]
        public  string? UserId { get; set; }
        public  AppUserDTO? User { get; set; }
    }
}
