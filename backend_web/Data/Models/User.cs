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
        public string? Major { get; set; }


        public ICollection<LostItem> LostItems { get; set; }

        // علاقة الصور
        public ICollection<UserImage> Images { get; set; } = new List<UserImage>();



        //list of products
        public List<Product> products { get; set; } = new List<Product>();
        public ICollection<LikedProduct> LikedProducts { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Comment> comments { get; set; }
        public ICollection<BookExchange> BookExchangePosts { get; set; } // علاقته بالمنشورات


    }
}
