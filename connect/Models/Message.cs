namespace connect.Models;

public class Message
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public bool Read { get; set; } = false;
    public string SenderName { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
}
