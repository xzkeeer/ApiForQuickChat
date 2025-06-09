// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models; // Замените на ваше пространство имён

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор с настройками
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Таблицы в БД
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserChat> UserChats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи многие-ко-многим (User ↔ Chat через UserChat)
            modelBuilder.Entity<UserChat>()
                .HasKey(uc => new { uc.UserId, uc.ChatId }); // Составной ключ

            // Связь User ↔ UserChat
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uc => uc.UserId);

            // Связь Chat ↔ UserChat
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatId);

            // Связь Chat → Message (один-ко-многим)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            // Связь User → Message (отправитель)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Запрет каскадного удаления
            modelBuilder.Entity<User>()
    .Property(u => u.PasswordHash)
    .HasColumnType("bytea");

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordSalt)
                .HasColumnType("bytea");
        }
    }
}