namespace NewsAPI.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public ICollection<Comment>? Comments { get; set; }
    }
}
