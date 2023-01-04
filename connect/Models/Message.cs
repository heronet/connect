namespace connect.Models;

public class Message
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    public Guid LastMessageId { get; set; }
    public Message LastMessage { get; set; }
}
