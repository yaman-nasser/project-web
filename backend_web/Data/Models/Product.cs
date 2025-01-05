using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace backend_web.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? caption { get; set; }
        public decimal price { get; set; }
        //type of product.........
        public string? type { get; set; }
        //+++++ phnum == phon-User...
        public string? PHnum { get; set; }
        //list of images
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
        public ICollection<LikedProduct> LikedProducts { get; set; }


        //[ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } // علاقة Many-to-One مع المستخدم
    }
}
