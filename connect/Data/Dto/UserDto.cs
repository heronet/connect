namespace connect.Data.Dto;

public class UserDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public PhotoDto Avatar { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastOnline { get; set; }
    public IFormFile UploadAvatar { get; set; }
}
