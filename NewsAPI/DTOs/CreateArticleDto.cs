namespace NewsAPI.DTOs
{
    public class CreateArticleDto
    {
        public required string Title { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
