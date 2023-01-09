using Microsoft.AspNetCore.Identity;

namespace connect.Models;

public class User : IdentityUser
{
    public string Name { get; set; }
    public DateTime LastOnline { get; set; } = DateTime.UtcNow;
    public ICollection<Chat> Chats { get; set; }
}
