using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace AnalyticsService.Services
{
    /// <summary>
    /// Detects high traffic on URLs
    /// Publishes alerts to RabbitMQ queue
    /// </summary>
    public class TrafficMonitorService
    {
        private readonly IConnection _rabbitMqConnection;

        public TrafficMonitorService(IConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
        }

        public void CheckTraffic(string shortUrl, int clickCount)
        {
            if (clickCount > 1000) // Example threshold
            {
                PublishAlert(new AdminAlert
                {
                    Type = "HighTraffic",
                    Message = $"URL {shortUrl} received {clickCount} clicks!",
                    Timestamp = DateTime.UtcNow,
                    SendEmail = true // Indicate that an email should be sent
                });
            }
        }

        private void PublishAlert(AdminAlert alert)
        {
            using var channel = _rabbitMqConnection.CreateModel();
            channel.QueueDeclare(queue: "admin_alerts", durable: true, exclusive: false, autoDelete: false);

            var message = JsonConvert.SerializeObject(alert);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "admin_alerts", basicProperties: null, body: body);
        }
    }
}

public class AdminAlert
{
    public string Type { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public bool SendEmail { get; set; }
}
