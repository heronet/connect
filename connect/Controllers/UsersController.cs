using System.Security.Claims;
using connect.Data;
using connect.Data.Dto;
using connect.Models;
using connect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connect.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly PhotoService _photoService;
    public UsersController(UserManager<User> userManager, ApplicationDbContext dbContext, PhotoService photoService)
    {
        _photoService = photoService;
        _dbContext = dbContext;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .Include(u => u.Avatar)
            .ToListAsync();
        var userDtos = users.Select(u => UserToDto(u)).ToList();
        return Ok(users);
    }
    [HttpGet("connected")]
    public async Task<ActionResult> GetConnectedUsers()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        var chats = await _dbContext.Chats
            .Include(c => c.Users)
            .ThenInclude(u => u.Avatar)
            .Where(c => c.Users.Contains(user) && c.Users.Count <= 2)
            .ToListAsync();
        var userDtos = chats
            .Select(c => c.Users.FirstOrDefault(u => u.Id != userId))
            .Select(u => UserToDto(u))
            .ToList();
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
            return Ok(UserToDto(recipient));
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
    [HttpPut("update")]
    public async Task<ActionResult> UpdateUser([FromForm] UserDto userDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.Users
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return BadRequest("User not found");
        // Photo upload
        if (userDto.UploadAvatar.Length > 0)
        {
            if (user.Avatar is not null)
            {
                var deletionResult = await _photoService.DeletePhotoAsync(user.Avatar.PublicId);
                user.Avatar = null;
            }
            var photoResult = await _photoService.AddAvatarAsync(userDto.UploadAvatar);
            if (photoResult.Error != null)
                return BadRequest(photoResult.Error.Message);
            var newPhoto = new Photo
            {
                ImageUrl = photoResult.SecureUrl.AbsoluteUri,
                PublicId = photoResult.PublicId
            };
            user.Avatar = newPhoto;
        }
        user.Name = userDto.Name?.Trim() ?? user.Name;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return Ok(UserToDto(user));
        return BadRequest(result.Errors);
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
}
