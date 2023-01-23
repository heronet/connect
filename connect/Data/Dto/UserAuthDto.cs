namespace connect.Data.Dto;

public class UserAuthDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string UserAvatarUrl { get; set; }
    public List<string> Roles { get; set; }
}
