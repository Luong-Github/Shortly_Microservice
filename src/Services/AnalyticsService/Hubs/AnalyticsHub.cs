using Microsoft.AspNetCore.SignalR;

namespace AnalyticsService.Hubs
{
    public class AnalyticsHub : Hub
    {
        public async Task SubscribeAnalytics(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task UnsubscribeAnalytics(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
    }
}
