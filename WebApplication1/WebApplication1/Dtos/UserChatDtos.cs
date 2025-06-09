namespace WebApplication1.Dtos
{
    // добавление пользователя в чат
    public class AddUserToChatDto
    {
        public required int UserId { get; set; }
        public required int ChatId { get; set; }
    }

    // ответ с информацией о связи
    public class UserChatResponseDto
    {
        public required UserResponseDto User { get; set; }
        public required ChatResponseDto Chat { get; set; }
    }

    // для массового добавления
    public class AddUsersToChatDto
    {
        public required int ChatId { get; set; }
        public required List<int> UserIds { get; set; }
    }
}