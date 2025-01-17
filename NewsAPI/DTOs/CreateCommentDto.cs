namespace NewsAPI.DTOs
{
    public class CreateCommentDto
    {
        public required string Content { get; set; } = string.Empty;
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
    }
}
