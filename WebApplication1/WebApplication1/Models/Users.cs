using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public User()
    {
        UserChats = new HashSet<UserChat>();
        SentMessages = new HashSet<Message>();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Username { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    public byte[] AvatarBytes { get; set; }
    public string AvatarMimeType { get; set; }

    public DateTime? LastOnline { get; set; }

    public bool IsOnline { get; set; } = false;

    public virtual ICollection<UserChat> UserChats { get; set; }
    public virtual ICollection<Message> SentMessages { get; set; }
}