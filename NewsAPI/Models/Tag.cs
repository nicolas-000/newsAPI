namespace NewsAPI.Models
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ICollection<ArticleTag>? ArticleTags { get; set; } = new List<ArticleTag>();
    }
}
