using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AnalyticsService.Hubs
{
    [Route("/login-hub")]
    public class LoginAnalyticsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveLoginUpdate", $"{ Context.ConnectionId } has joined" );
        }
        //Defines a SignalR hub that sends login updates to all connected clients.
        public async Task SendLoginUpdate(string userId)
        {
            await Clients.All.SendAsync("ReceiveLoginUpdate", userId);
        }
    }
}
