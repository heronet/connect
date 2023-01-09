using System.ComponentModel.DataAnnotations;

namespace connect.Data.Dto;

public class ConnectUserDto
{
    [Required]
    public string RecipientId { get; set; }
}
