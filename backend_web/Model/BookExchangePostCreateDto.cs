namespace backend_web.Model
{
    public class BookExchangePostCreateDto
    {
        public int UserId { get; set; }
        public string BooksToGive { get; set; }
        public string BooksToReceive { get; set; }

    }
}
