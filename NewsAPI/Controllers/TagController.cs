using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.DTOs;
using NewsAPI.Models;

namespace NewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController(ApiContext context) : ControllerBase
    {
        private readonly ApiContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            return await _context.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);

            return tag == null ? (ActionResult<TagDto>)NotFound() : (ActionResult<TagDto>)new TagDto { Id = tag.Id, Name = tag.Name };
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> PostTag(CreateTagDto tagDto)
        {
            if (tagDto == null)
            {
                return BadRequest("Tag cannot be null.");
            }

            if (await _context.Tags.AnyAsync(t => t.Name.ToLower() == tagDto.Name.ToLower()))
            {
                return Conflict("A tag with this name already exists.");
            }

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = tagDto.Name
            };

            _context.Tags.Add(tag);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while saving the tag.");
            }

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new TagDto { Id = tag.Id, Name = tag.Name });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(Guid id, CreateTagDto tagDto)
        {
            if (tagDto == null)
            {
                return BadRequest("Tag cannot be null.");
            }
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            tag.Name = tagDto.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the tag.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while deleting the tag.");
            }

            return NoContent();
        }
    }
}
