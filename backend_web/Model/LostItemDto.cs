using System.ComponentModel.DataAnnotations;

namespace backend_web.Model
{
    public class LostItemDto
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public List<IFormFile>? ImageFiles { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
