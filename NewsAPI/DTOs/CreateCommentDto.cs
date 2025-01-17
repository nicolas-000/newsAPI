namespace NewsAPI.DTOs
{
    public class CreateCommentDto
    {
        public required string Content { get; set; } = string.Empty;
        public required Guid ArticleId { get; set; }
        public required Guid UserId { get; set; }
    }
}
