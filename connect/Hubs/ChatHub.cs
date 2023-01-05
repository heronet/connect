using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace connect.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async override Task OnConnectedAsync()
    {
        var username = Context.User.FindFirst(ClaimTypes.Name).Value;
        await Clients.All.SendAsync("connected", username);
    }
    public async override Task OnDisconnectedAsync(Exception exception)
    {
        var username = Context.User.FindFirst(ClaimTypes.Name).Value;
        await Clients.All.SendAsync("Disconnected", username);
        await base.OnDisconnectedAsync(exception);
    }
}
