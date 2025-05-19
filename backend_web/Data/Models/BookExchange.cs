namespace backend_web.Data.Models
{
    public class BookExchange
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        


        public string? BooksToGive { get; set; }   // الكتب التي يريد إعطاءها
        public string? BooksToReceive { get; set; } // المواد التي يريد الحصول عليها

        public string? Major { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
