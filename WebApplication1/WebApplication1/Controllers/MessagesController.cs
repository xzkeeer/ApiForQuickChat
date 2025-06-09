using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetMessages(int chatId)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageResponseDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Sender = new UserResponseDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        AvatarUrl = $"/api/users/avatar/{m.Sender.Id}",
                        IsOnline = m.Sender.IsOnline
                    },
                    Text = m.Text,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<MessageResponseDto>> SendMessage(SendMessageDto dto)
        {
            var message = new Message
            {
                ChatId = dto.ChatId,
                SenderId = dto.SenderId,
                Text = dto.Text,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var sender = await _context.Users.FindAsync(message.SenderId);

            return new MessageResponseDto
            {
                Id = message.Id,
                Sender = new UserResponseDto
                {
                    Id = sender.Id,
                    Username = sender.Username,
                    AvatarUrl = $"/api/users/avatar/{sender.Id}",
                    IsOnline = sender.IsOnline
                },
                Text = message.Text,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };
        }
    }
}