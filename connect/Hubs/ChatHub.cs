using Microsoft.AspNetCore.SignalR;

namespace connect.Hubs;

public class ChatHub : Hub
{
    public async override Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("connected", "Connected");
    }
}
