namespace connect.Models;

public class Comment
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public string UserName { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid PostId { get; set; }
    public Post Post { get; set; }
}
