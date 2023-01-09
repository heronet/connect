namespace connect.Data.Dto;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; }
    public bool Read { get; set; }
    public string SenderName { get; set; }
    public string UserId { get; set; }
    public Guid ChatId { get; set; }
}
