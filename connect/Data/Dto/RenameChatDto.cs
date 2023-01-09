using System.ComponentModel.DataAnnotations;

namespace connect.Data.Dto;

public class RenameChatDto
{
    [Required]
    public Guid ChatId { get; set; }
    [Required]
    public string ChatName { get; set; }
}
