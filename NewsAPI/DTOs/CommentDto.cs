namespace NewsAPI.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
