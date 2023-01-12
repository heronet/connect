using Microsoft.AspNetCore.Identity;

namespace connect.Models;

public class User : IdentityUser
{
    public string Name { get; set; }
    public DateTime LastOnline { get; set; }
    public ICollection<Chat> Chats { get; set; }
    public ICollection<Post> Posts { get; set; }
}
