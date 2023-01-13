namespace connect.Data.Dto;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public long LikesCount { get; set; }
    public long CommentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
}
