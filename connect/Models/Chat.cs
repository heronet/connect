namespace connect.Models;
public static class ChatType
{
    public static string O2O { get; set; } = "One2One";
    public static string Group { get; set; } = "Group";
}
public class Chat
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Titles { get; set; }
    public ICollection<User> Users { get; set; }
    public ICollection<Message> Messages { get; set; }
    public string LastMessage { get; set; }
    public string LastMessageSender { get; set; }
    public string LastMessageSenderId { get; set; }
    public DateTime LastMessageTime { get; set; }
    public string Type { get; set; }
}
