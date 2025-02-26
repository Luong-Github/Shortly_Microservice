using AnalyticsService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AnalyticsService.Services
{
    public class LoginTrackService
    {
        private readonly IHubContext<LoginAnalyticsHub> _hubContext;

        public LoginTrackService(IHubContext<LoginAnalyticsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task TrackLogin(string userId)
        {
            // Simulate storing login analytics in Redis or DB
            Console.WriteLine($"Tracking login for User: {userId}");

            // Send real-time update via WebSocket
            // Broadcasts login updates to all WebSocket clients.
            await _hubContext.Clients.All.SendAsync("ReceiveLoginUpdate", userId);
        }
    }
}
