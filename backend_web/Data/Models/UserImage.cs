using System.ComponentModel.DataAnnotations;

namespace backend_web.Data.Models
{
    public class UserImage
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } // اسم الصورة
        public byte[]? Image { get; set; } // البيانات الثنائية للصورة
        public string ContentType { get; set; } // نوع الصورة (image/png)

        // علاقة مع المستخدم
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
