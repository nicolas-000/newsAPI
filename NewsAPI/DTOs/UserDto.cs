using NewsAPI.Models;

namespace NewsAPI.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public List<CommentDto>? Comments { get; set; } = new List<CommentDto>();
    }
}
