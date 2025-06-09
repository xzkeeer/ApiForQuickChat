using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Dtos
{
    // создание чата
    public class CreateChatDto
    {
        public required string Name { get; set; }
        public bool IsGroup { get; set; }
        public required List<int> ParticipantIds { get; set; }
    }

    // ответ с данными чата
    public class ChatResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsGroup { get; set; }
        public List<UserResponseDto> Participants { get; set; } = new();
    }

    // краткая информация о чате (для списков)
    public class ChatShortInfoDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}
