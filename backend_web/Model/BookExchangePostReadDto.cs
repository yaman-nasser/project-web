namespace backend_web.Model
{
    public class BookExchangePostReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Major { get; set; } // ✅ عرض التخصص
        public string BooksToGive { get; set; }
        public string BooksToReceive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }

        public string? ProfileImageBase64 { get; set; } // ✅ الصورة على شكل Base64
        public string? ProfileImageContentType { get; set; } // ✅ نوع الصورة

        

    }
}
