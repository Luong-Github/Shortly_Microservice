using AnalyticsService.Hubs;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace AnalyticsService.Services
{
    public class AnalyticsService
    {
        private readonly IHubContext<AnalyticsHub> _hubContext;
        private readonly IConnectionMultiplexer _redis;

        public AnalyticsService(IHubContext<AnalyticsHub> hubContext, IConnectionMultiplexer redis)
        {
            _hubContext = hubContext;
            _redis = redis;
        }

        public async Task TrackUrlClick(string userId, string urlId)
        {
            var db = _redis.GetDatabase();
            var timestamp = DateTime.UtcNow.ToString("o");

            // Store analytics in Redis
            await db.ListRightPushAsync($"analytics:{userId}", $"{urlId}|{timestamp}");

            // Publish analytics event
            var pubSub = _redis.GetSubscriber();
            await pubSub.PublishAsync("analytics_updates", $"{userId}|{urlId}|{timestamp}");
        }

        public void SubscribeToAnalyticsUpdates()
        {
            var pubSub = _redis.GetSubscriber();
            pubSub.Subscribe("analytics_updates", async (channel, message) =>
            {
                var parts = message.ToString().Split('|');
                var userId = parts[0];
                var urlId = parts[1];
                var timestamp = parts[2];

                // Send analytics update via SignalR
                await _hubContext.Clients.Group(userId).SendAsync("ReceiveAnalyticsUpdate", new
                {
                    UrlId = urlId,
                    Timestamp = timestamp
                });
            });
        }
    }
}
