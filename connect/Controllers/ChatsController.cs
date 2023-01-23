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
public class ChatsController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    public ChatsController(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<ActionResult> GetChats()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.Users
            .Include(u => u.Chats)
            .ThenInclude(c => c.Users)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User does not exist");
        var chatDtos = user.Chats.Select(c => ChatToDto(c, userId)).ToList();
        return Ok(chatDtos);
    }
    [HttpGet("{chatId}")]
    public async Task<ActionResult> GetChat(Guid chatId, int skip = 0, int take = 20)
    {
        var chat = await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .Include(c => c.Users)
            .ThenInclude(u => u.Avatar)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync();
        if (chat == null) NotFound("Chat does not exists");
        chat.Messages = chat.Messages.OrderBy(m => m.Time).SkipLast(skip).TakeLast(take).ToList();
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (!chat.Users.Contains(user))
            return BadRequest("You are not in this chat");
        var chatDto = ChatToDto(chat, userId);
        return Ok(chatDto);
    }
    private PhotoDto PhotoToDto(Photo photo)
    {
        return new PhotoDto
        {
            Id = photo.Id,
            ImageUrl = photo.ImageUrl,
            PublicId = photo.PublicId
        };
    }

    private UserDto UserToDto(User user)
    {
        return new UserDto
        {
            Email = user.Email,
            Name = user.Name,
            Id = user.Id,
            Avatar = (user.Avatar != null) ? PhotoToDto(user.Avatar) : null
        };
    }
    private MessageDto MessageToDto(Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            Text = message.Text,
            Read = message.Read,
            Time = message.Time,
            SenderName = message.SenderName,
            UserId = message.UserId,
            ChatId = message.ChatId
        };
    }
    private ChatDto ChatToDto(Chat chat, string userId)
    {
        return new ChatDto
        {
            Id = chat.Id,
            Title = chat.Titles[userId],
            LastMessage = chat.LastMessage,
            LastMessageSender = chat.LastMessageSender,
            LastMessageSenderId = chat.LastMessageSenderId,
            LastMessageTime = chat.LastMessageTime,
            Type = chat.Type,
            AvatarUrl = chat.Users.FirstOrDefault(u => u.Id != userId).Avatar?.ImageUrl,
            Users = chat.Users.Select(u => UserToDto(u)).ToList(),
            Messages = chat.Messages?.Select(m => MessageToDto(m)).ToList()
        };
    }
}
