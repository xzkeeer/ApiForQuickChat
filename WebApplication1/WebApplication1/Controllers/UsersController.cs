using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            return await _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    AvatarUrl = $"/api/users/avatar/{u.Id}",
                    IsOnline = u.IsOnline,
                    LastOnline = u.LastOnline
                })
                .ToListAsync();
        }

        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .Where(u => u.Username.ToLower() == username.ToLower())
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    AvatarUrl = $"/api/users/avatar/{u.Id}",
                    IsOnline = u.IsOnline,
                    LastOnline = u.LastOnline
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("avatar/{userId}")]
        public async Task<IActionResult> GetAvatar(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.AvatarBytes == null)
                return NotFound();

            return File(user.AvatarBytes, user.AvatarMimeType);
        }

        [HttpPut("avatar")]
        public async Task<ActionResult<UserResponseDto>> UpdateAvatar([FromBody] UpdateAvatarDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return NotFound();

            try
            {
                var (bytes, mimeType) = ConvertBase64ToBytes(dto.AvatarBase64);
                user.AvatarBytes = bytes;
                user.AvatarMimeType = mimeType;

                await _context.SaveChangesAsync();

                return new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    AvatarUrl = $"/api/users/avatar/{user.Id}",
                    IsOnline = user.IsOnline,
                    LastOnline = user.LastOnline
                };
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving avatar: {ex.Message}");
            }
        }

        private (byte[] bytes, string mimeType) ConvertBase64ToBytes(string base64String)
        {
            var base64Data = base64String.Contains(",") ?
                base64String.Split(',')[1] :
                base64String;

            var bytes = Convert.FromBase64String(base64Data);

            string mimeType = "image/jpeg";
            if (base64String.StartsWith("data:image/png"))
                mimeType = "image/png";
            else if (base64String.StartsWith("data:image/jpeg") || base64String.StartsWith("data:image/jpg"))
                mimeType = "image/jpeg";
            else if (base64String.StartsWith("data:image/gif"))
                mimeType = "image/gif";

            return (bytes, mimeType);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query is empty");

            var users = await _context.Users
                .Where(u => u.Username.Contains(query))
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    AvatarUrl = $"/api/users/avatar/{u.Id}",
                    IsOnline = u.IsOnline,
                    LastOnline = u.LastOnline
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}