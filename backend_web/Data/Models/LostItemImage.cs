using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend_web.Data.Models
{
    public class LostItemImage
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[]? Image { get; set; }

        public string ContentType { get; set; }

        // العلاقة مع LostItem
        [Required]
        public int LostItemId { get; set; }

        [ForeignKey("LostItemId")]
        public LostItem LostItem { get; set; }
    }
}
