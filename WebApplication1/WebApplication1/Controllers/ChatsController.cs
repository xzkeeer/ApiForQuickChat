using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatResponseDto>>> GetUserChats(int userId)
        {
            return await _context.UserChats
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Chat)
                .ThenInclude(c => c.UserChats)
                .ThenInclude(uc => uc.User)
                .Select(uc => new ChatResponseDto
                {
                    Id = uc.Chat.Id,
                    Name = uc.Chat.Name,
                    IsGroup = uc.Chat.IsGroup,
                    Participants = uc.Chat.UserChats
                        .Select(u => new UserResponseDto
                        {
                            Id = u.User.Id,
                            Username = u.User.Username,
                            AvatarUrl = $"/api/users/avatar/{u.User.Id}",
                            IsOnline = u.User.IsOnline
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> CreateChat(CreateChatDto dto)
        {
            var chat = new Chat
            {
                Name = dto.Name,
                IsGroup = dto.IsGroup
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            foreach (var userId in dto.ParticipantIds)
            {
                _context.UserChats.Add(new UserChat
                {
                    UserId = userId,
                    ChatId = chat.Id
                });
            }

            await _context.SaveChangesAsync();

            var participants = await _context.Users
                .Where(u => dto.ParticipantIds.Contains(u.Id))
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    AvatarUrl = $"/api/users/avatar/{u.Id}",
                    IsOnline = u.IsOnline
                })
                .ToListAsync();

            return new ChatResponseDto
            {
                Id = chat.Id,
                Name = chat.Name,
                IsGroup = chat.IsGroup,
                Participants = participants
            };
        }

    }

}