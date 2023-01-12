namespace connect.Models;

public class Like
{
    public Guid Id { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid PostId { get; set; }
    public Post Post { get; set; }
}
