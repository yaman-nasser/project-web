namespace backend_web.Model
{
    public class CommentCreateDto
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }

    public class CommentReadDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } // أو Email
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
