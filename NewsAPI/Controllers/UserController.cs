using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.DTOs;
using NewsAPI.Models;

namespace NewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ApiContext context) : ControllerBase
    {
        private readonly ApiContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return await _context.Users.Include(u => u.Comments).Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Comments = u.Comments!.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Username = c.User.Username,
                    Content = c.Content
                }).ToList()
            }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _context.Users.Include(u => u.Comments).FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? (ActionResult<UserDto>)NotFound() : (ActionResult<UserDto>)new UserDto { Id = user.Id, Username = user.Username, Comments = user.Comments!.Select(c => new CommentDto
            {
                Id = c.Id,
                Username = c.User.Username,
                Content = c.Content
            }).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(CreateUserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User cannot be null");
            }

            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == userDto.Username.ToLower()))
            {
                return Conflict("A user with this username already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userDto.Username
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while saving the user.");
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto { Id = user.Id, Username = user.Username });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, CreateUserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = userDto.Username;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the user.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }

            return NoContent();
        }
    }
}
