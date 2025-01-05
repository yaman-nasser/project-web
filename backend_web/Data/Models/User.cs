using System.ComponentModel.DataAnnotations;

namespace backend_web.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? Email { get; set; }
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{8,15}$")]
        public string? Password { get; set; }
        [RegularExpression(@"^(077|078|079)\d{7}$")]
        public string? Phone { get; set; }

        //list of products
        public List<Product> products { get; set; } = new List<Product>();
        public ICollection<LikedProduct> LikedProducts { get; set; }

    }
}
