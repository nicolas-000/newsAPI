using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.DTOs;
using NewsAPI.Models;

namespace NewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController(ApiContext context) : ControllerBase
    {
        private readonly ApiContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
        {
            return await _context.Articles
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    Tags = a.ArticleTags.Select(at => new TagDto
                    {
                        Id = at.Tag.Id,
                        Name = at.Tag.Name
                    }).ToList()
                }).ToListAsync();
        }

        [HttpGet("{articleId}")]
        public async Task<ActionResult<ArticleDto>> GetArticle(Guid articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);

            if (article == null)
            {
                return NotFound();
            }

            var articleDto = new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CreatedAt = article.CreatedAt,
                Tags = article.ArticleTags.Select(at => new TagDto
                {
                    Id = at.Tag.Id,
                    Name = at.Tag.Name
                }).ToList(),
                Comments = article.Comments!.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Username = c.User.Username,
                    Content = c.Content,
                }).ToList()
            };

            return articleDto;
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDto>> PostArticle(CreateArticleDto articleDto)
        {
            if (articleDto == null)
            {
                return BadRequest("Article cannot be null");
            }

            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = articleDto.Title,
                Content = articleDto.Content,
                CreatedAt = DateTime.Now
            };

            _context.Articles.Add(article);

            var tags = articleDto.Tags;

            foreach (var tag in tags)
            {
                var articleTag = new ArticleTag
                {
                    ArticleId = article.Id,
                    Article = article,
                    TagId = tag.Id,
                    Tag = _context.Tags.Find(tag.Id)
                };

                _context.ArticleTags.Add(articleTag);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while saving the article.");
            }

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, new ArticleDto { Id = article.Id, Title = article.Title, Content = article.Content, CreatedAt = article.CreatedAt, Tags = tags });
        }

        [HttpPut("{articleId}")]
        public async Task<IActionResult> PutArticle(Guid articleId, CreateArticleDto articleDto)
        {
            if (articleDto == null)
            {
                return BadRequest("Article cannot be null");
            }

            var article = await _context.Articles.FindAsync(articleId);

            if (article == null)
            {
                return NotFound();
            }

            article.Title = articleDto.Title;
            article.Content = articleDto.Content;

            var articlesTags = _context.ArticleTags.Where(at => at.ArticleId == article.Id).ToList();

            _context.ArticleTags.RemoveRange(articlesTags);

            foreach (var tagDto in articleDto.Tags)
            {
                var articleTag = new ArticleTag
                {
                    ArticleId = article.Id,
                    Article = article,
                    TagId = tagDto.Id,
                    Tag = _context.Tags.Find(tagDto.Id)
                };

                _context.ArticleTags.Add(articleTag);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the article.");
            }

            return NoContent();
        }

        [HttpDelete("{articleId}")]
        public async Task<IActionResult> DeleteArticle(Guid articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while deleting the article.");
            }

            return NoContent();
        }

        // Comments
        [HttpGet("{articleId}/comments")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(Guid articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.Where(c => c.ArticleId == articleId).ToListAsync();

            if (comments.Count == 0) {
                return Content(string.Empty);
            }

            return comments.ConvertAll(c => new CommentDto
            {
                Id = c.Id,
                Username = c.User.Username,
                Content = c.Content
            });
        }

        [HttpGet("{articleId}/comments/{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(Guid articleId, Guid id)
        {
            var article = await _context.Articles.FindAsync(articleId);
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || article == null)
            {
                return NotFound();
            }

            return new CommentDto { Id = comment.Id, Username = comment.User.Username, Content = comment.Content };
        }

        [HttpPost("{articleId}/comments")]
        public async Task<ActionResult<CommentDto>> PostComment(Guid articleId, CreateCommentDto commentDto)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = commentDto.Content,
                ArticleId = commentDto.ArticleId,
                Article = article,
                UserId = commentDto.UserId,
                User = _context.Users.Find(commentDto.UserId)
            };

            _context.Comments.Add(comment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while saving the comment.");
            }

            return CreatedAtAction(nameof(GetComment), new { articleId = articleId, id = comment.Id }, new CommentDto { Id = comment.Id, Username = comment.User.Username, Content = comment.Content });
        }

        [HttpPut("{articleId}/comments/{id}")]
        public async Task<IActionResult> PutComment(Guid articleId, Guid id, CreateCommentDto commentDto)
        {
            var article = await _context.Articles.FindAsync(articleId);
            var comment = await _context.Comments.FindAsync(id);
            if (article == null || comment == null)
            {
                return NotFound();
            }

            comment.Content = commentDto.Content;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the comment.");
            }

            return NoContent();
        }

        [HttpDelete("{articleId}/comments/{id}")]
        public async Task<IActionResult> DeleteComment(Guid articleId, Guid id, CreateCommentDto commentDto)
        {
            var article = await _context.Articles.FindAsync(articleId);
            var comment = await _context.Comments.FindAsync(id);
            if (article == null || comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while deleting the comment.");
            }

            return NoContent();
        }
    }
}
