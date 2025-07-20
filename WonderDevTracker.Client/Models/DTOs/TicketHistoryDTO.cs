using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Client.Models.DTOs
{
    public class TicketHistoryDTO
    {
        private DateTimeOffset _created;

       [Required]
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        //navigation properties
        
        [Required]
        public string? UserId { get; set; }
        public  AppUserDTO? User { get; set; }
    }
}