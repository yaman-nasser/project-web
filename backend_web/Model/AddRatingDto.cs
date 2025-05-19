namespace backend_web.Model
{
    public class AddRatingDto
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // من 1 إلى 5
    }
}
