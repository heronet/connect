using Microsoft.AspNetCore.Identity;

namespace connect.Models;

public class User : IdentityUser
{
    public string Name { get; set; }
    public Photo Avatar { get; set; }
    public string Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastOnline { get; set; }
    public ICollection<Chat> Chats { get; set; }
    public ICollection<Post> Posts { get; set; }
}
