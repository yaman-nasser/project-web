using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend_web.Data.Models
{
    public class LostItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } // "ضائع" أو "تم العثور عليه"

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public byte[]? Image { get; set; }

        public ICollection<LostItemImage> Images { get; set; }
        public string Location { get; set; }

        public DateTime Date { get; set; }

        public string PhoneNumber { get; set; }

        // الربط بالمستخدم
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } // أفترض أن لديك جدول User
    }
}
