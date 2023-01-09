using System.ComponentModel.DataAnnotations;

namespace connect.Data.Dto;

public class RegisterDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
