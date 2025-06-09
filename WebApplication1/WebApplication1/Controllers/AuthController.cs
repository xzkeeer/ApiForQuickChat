using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(RegisterUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key,
                IsOnline = true,
                LastOnline = DateTime.UtcNow
            };

            if (!string.IsNullOrEmpty(dto.AvatarBase64))
            {
                var (bytes, mimeType) = ConvertBase64ToBytes(dto.AvatarBase64);
                user.AvatarBytes = bytes;
                user.AvatarMimeType = mimeType;
            }

            _context.Users.Add(user);
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
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login(LoginUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized("Invalid password");

            user.IsOnline = true;
            user.LastOnline = DateTime.UtcNow;
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
    }
}