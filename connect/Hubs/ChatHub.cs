using System.Security.Claims;
using connect.Data;
using connect.Data.Dto;
using connect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace connect.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    public ChatHub(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async override Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        await Clients.User(userId).SendAsync("Connected", userId);
    }
    public async Task GetChats()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new HubException("User does not exist");
        var chats = await _dbContext.Chats
            .Include(c => c.Users)
            .Where(c => c.Users.Contains(user))
            .ToListAsync();
        var chatDtos = chats.Select(c => ChatToDto(c, userId)).ToList();
        await Clients.User(userId).SendAsync("ReceivedChats", chatDtos);
    }
    public async Task GetChat(Guid id)
    {
        var chat = await _dbContext.Chats
            .Where(c => c.Id == id)
            .Include(c => c.Users)
            .Include(c => c.Messages.OrderBy(m => m.Time))
            .FirstOrDefaultAsync();
        if (chat == null) throw new HubException("Chat does not exists");
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var chatDto = ChatToDto(chat, userId);
        await Clients.Client(Context.ConnectionId).SendAsync("ReceivedChat", chatDto);
    }

    public async Task SendMessage(MessageDto messageDto)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);

        var chat = await _dbContext.Chats
            .Where(c => c.Id == messageDto.ChatId)
            .Include(c => c.Users)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync();
        if (chat == null)
            throw new HubException("Chat doesnot exist");
        var message = new Message
        {
            Text = messageDto.Text,
            Read = false,
            User = user,
            SenderName = user.Name
        };
        chat.Messages.Add(message);
        chat.LastMessage = message.Text;
        chat.LastMessageSender = user.Name;
        _dbContext.Chats.Update(chat);
        if (await _dbContext.SaveChangesAsync() > 0)
        {
            var sendMessageDto = MessageToDto(message);
            foreach (var u in chat.Users)
            {
                await Clients.User(u.Id).SendAsync("ReceivedMessage", sendMessageDto);
            }
        }
    }
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        user.LastOnline = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await Clients.User(userId).SendAsync("Disconnected", userId);
        await base.OnDisconnectedAsync(exception);
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
            Type = chat.Type,
            Users = chat.Users.Select(u => UserToDto(u)).ToList(),
            Messages = chat.Messages?.Select(m => MessageToDto(m)).ToList()
        };
    }
    private UserDto UserToDto(User user)
    {
        return new UserDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Id = user.Id
        };
    }
}
