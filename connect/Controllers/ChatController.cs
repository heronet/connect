using System.Security.Claims;
using connect.Data;
using connect.Data.Dto;
using connect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connect.Controllers;

[Authorize]
public class ChatController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    public ChatController(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }
    [HttpGet]
    public async Task<ActionResult> GetChats()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");
        var chats = await _dbContext.Chats
            .Include(c => c.Users)
            .Where(c => c.Users.Contains(user))
            .ToListAsync();
        var chatDtos = chats.Select(c => ChatToDto(c)).ToList();
        return Ok(chatDtos);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult> GetChat(Guid id)
    {
        var chat = await _dbContext.Chats
            .Where(c => c.Id == id)
            .Include(c => c.Users)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync();
        if (chat == null) return BadRequest("Chat not found");
        return Ok(ChatToDto(chat));
    }
    private ChatDto ChatToDto(Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            LastMessage = chat.LastMessage,
            Type = chat.Type,
            Users = chat.Users.Select(u => UserToDto(u)).ToList(),
            Messages = chat.Messages?.Select(m => MessageToDto(m)).ToList()
        };
    }
    private UserDto UserToDto(User user)
    {
        return new UserDto
        {
            Email = user.Email
        };
    }
    private MessageDto MessageToDto(Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            Text = message.Text,
            Read = message.Read,
            UserId = message.UserId,
            ChatId = message.ChatId
        };
    }
}
