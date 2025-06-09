namespace WebApplication1.Dtos
{
    // для регистрации
    public class RegisterUserDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string AvatarBase64 { get; set; } // Изменили с AvatarUrl на AvatarBase64
    }

    // для входа
    public class LoginUserDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class UpdateAvatarDto
    {
        public int UserId { get; set; }
        public string AvatarBase64 { get; set; }
    }

    // ответ с данными пользователя
    public class UserResponseDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string AvatarUrl { get; set; } // URL для доступа к аватару через API
        public bool IsOnline { get; set; }
        public DateTime? LastOnline { get; set; }
    }
}