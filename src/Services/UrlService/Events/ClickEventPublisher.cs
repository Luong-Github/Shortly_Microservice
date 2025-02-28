using Microsoft.AspNetCore.Connections;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace UrlService.Events
{
    public class ClickEventPublisher
    {
        private readonly ConnectionFactory _factory;

        public ClickEventPublisher()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public void PublishClickEvent(string userId, string urlId)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "click_events", durable: true, exclusive: false, autoDelete: false);

            var clickEvent = new { UserId = userId, UrlId = urlId, Timestamp = DateTime.UtcNow };
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clickEvent));

            channel.BasicPublish(exchange: "", routingKey: "click_events", basicProperties: null, body: messageBody);

            Console.WriteLine($"[x] Sent Click Event: {clickEvent}");
        }
    }
}
