namespace WebApplication1.Dtos
{
    // отправка сообщения
    public class SendMessageDto
    {
        public required string Text { get; set; }
        public required int ChatId { get; set; }
        public int SenderId { get; set; }
    }

    // ответ с сообщением
    public class MessageResponseDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public required string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public required UserResponseDto Sender { get; set; }


    }

    // обновление статуса прочтения
    public class UpdateMessageStatusDto
    {
        public required List<int> MessageIds { get; set; }
        public bool IsRead { get; set; }
    }

}