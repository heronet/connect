namespace connect.Data.Dto;

public class ChatDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ICollection<UserDto> Users { get; set; }
    public ICollection<MessageDto> Messages { get; set; }
    public string LastMessage { get; set; }
    public string LastMessageSender { get; set; }
    public string Type { get; set; }
}
