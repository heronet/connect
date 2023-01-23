namespace connect.Data.Dto;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserAvatarUrl { get; set; }
    public Guid PostId { get; set; }
}