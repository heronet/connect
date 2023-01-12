namespace connect.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; }
    public User User { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Photo> Photos { get; set; }
}
