using Microsoft.AspNetCore.Identity;

namespace connect.Models;

public class User : IdentityUser
{
    public ICollection<Chat> Chats { get; set; }
}
