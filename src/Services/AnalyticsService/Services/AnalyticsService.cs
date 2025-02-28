using AnalyticsService.Hubs;
using AnalyticsService.Models;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace AnalyticsService.Services
{
    public class AnalyticsService
    {
        private readonly IHubContext<AnalyticsHub> _hubContext;
        private readonly IConnectionMultiplexer _redis;
        private readonly ConnectionFactory _factory;

        public AnalyticsService(IHubContext<AnalyticsHub> hubContext, IConnectionMultiplexer redis )
        {
            _hubContext = hubContext;
            _redis = redis;

            _factory = new ConnectionFactory() { HostName = "localhost" };

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

        public void StartListening()
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "click_events", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var clickEvent = JsonSerializer.Deserialize<ClickRecord>(message);

                Console.WriteLine($"[x] Received Click Event: {message}");

                // Store in Redis
                var db = _redis.GetDatabase();
                await db.ListRightPushAsync($"analytics:{clickEvent.UserId}", $"{clickEvent.ShortCode}|{clickEvent.CreatedDate}");

                // Notify SignalR clients
                await _hubContext.Clients.Group(clickEvent.UserId).SendAsync("ReceiveAnalyticsUpdate", clickEvent);
            };

            channel.BasicConsume(queue: "click_events", autoAck: true, consumer: consumer);
        }
    }
}
