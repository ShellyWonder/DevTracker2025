using System.ComponentModel.DataAnnotations;

namespace WonderDevTracker.Models
{
    public class ImageUpload
    {
        public Guid Id { get; set; }

        [Required]
        public byte[]? Data { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        //calculated property
        public string Url => $"/uploads/{Id}";
    }
}
