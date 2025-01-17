namespace NewsAPI.DTOs
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
        public List<CommentDto>? Comments { get; set; } = new List<CommentDto>();
    }
}
