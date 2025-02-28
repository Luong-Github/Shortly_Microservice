using AnalyticsService.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using AnalyticsService.Models;

namespace AnalyticsService.Events
{
    public class ClickEventConsumer
    {
        private readonly IHubContext<AnalyticsHub> _hubContext;
        private readonly IConnectionMultiplexer _redis;
        private readonly ConnectionFactory _factory;

        public ClickEventConsumer(IHubContext<AnalyticsHub> hubContext, IConnectionMultiplexer redis)
        {
            _hubContext = hubContext;
            _redis = redis;
            _factory = new ConnectionFactory() { HostName = "localhost" };
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
