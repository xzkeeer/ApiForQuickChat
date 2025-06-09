using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

    public class UserChat
    {
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
    }

