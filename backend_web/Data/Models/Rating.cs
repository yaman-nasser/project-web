using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace backend_web.Data.Models
{
    public class Rating
    {
       
        public int Id { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int RatingValue { get; set; } // 1 to 5

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
