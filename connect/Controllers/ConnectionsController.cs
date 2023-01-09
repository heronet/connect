using System.Security.Claims;
using connect.Data;
using connect.Data.Dto;
using connect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace connect.Controllers;

[Authorize]
public class ConnectionsController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    public ConnectionsController(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .Select(u => new UserDto { Email = u.Email, Id = u.Id })
            .ToListAsync();
        return Ok(users);
    }

    [HttpGet("connected")]
    public async Task<ActionResult> GetConnectedUsers()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);

        var userDtos = await _dbContext.Chats
            .Include(c => c.Users)
            .Where(c => c.Users.Contains(user) && c.Users.Count <= 2)
            .Select(c => c.Users.FirstOrDefault(u => u.Id != userId))
            .Select(u => new UserDto { Id = u.Id, Email = u.Email })
            .ToListAsync();

        return Ok(userDtos);
    }
    [HttpPost("connect")]
    public async Task<ActionResult> ConnectUser(ConnectUserDto connectUserDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var recipientId = connectUserDto.RecipientId;
        if (userId == recipientId) return BadRequest("Cannot connect to self");

        var user = await _userManager.FindByIdAsync(userId);
        var recipient = await _userManager.FindByIdAsync(recipientId);
        if (recipient == null) return BadRequest("Target does not exist");

        var chat = await _dbContext.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Users.Contains(user) && c.Users.Contains(recipient) && c.Users.Count <= 2);
        if (chat != null)
            return BadRequest("Connection already exists");

        chat = new Chat
        {
            Messages = new List<Message>(),
            Users = new List<User> { user, recipient },
            Titles = new Dictionary<string, string> { { userId, recipient.Name }, { recipientId, user.Name } }
        };
        _dbContext.Chats.Add(chat);
        if (await _dbContext.SaveChangesAsync() > 0)
        {
            return RedirectToAction(nameof(GetConnectedUsers));
        }
        return BadRequest("Connection error");
    }

    [HttpPatch("rename")]
    public async Task<ActionResult> RenameChat(RenameChatDto renameChatDto)
    {
        var chatId = renameChatDto.ChatId;
        var chatName = renameChatDto.ChatName;

        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User does not exist");

        var chat = await _dbContext.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            return BadRequest("Connection already exists");
        if (!chat.Users.Contains(user))
            return BadRequest("You are not permitted to rename the chat");

        chat.Titles[userId] = chatName;
        _dbContext.Chats.Update(chat);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            return Ok(new ChatDto { Title = chatName });
        }
        return BadRequest("Chat rename error");
    }

    [HttpGet("connected/{recipientId}")]
    public async Task<ActionResult> GetChatId(string recipientId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (userId == recipientId) return BadRequest("Cannot connect to self");

        var user = await _userManager.FindByIdAsync(userId);
        var recipient = await _userManager.FindByIdAsync(recipientId);
        if (recipient == null) return BadRequest("Target does not exist");

        var chat = await _dbContext.Chats
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Users.Contains(user) && c.Users.Contains(recipient) && c.Users.Count <= 2);
        if (chat == null)
            return BadRequest("Connection doesnot exists");

        var chatDto = new ChatDto
        {
            Id = chat.Id
        };

        return Ok(chatDto);
    }
}
