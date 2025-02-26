
using AnalyticsService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AnalyticsService.Services
{
    public class RabbitMqConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitRabbitMq();
        }

        private void InitRabbitMq()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "user-logged-in",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var userLoggedInEvent = UserLoggerInEvent.FromJson(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var redisService = scope.ServiceProvider.GetRequiredService<RedisService>();
                    await redisService.TrackUserLoginAsync(userLoggedInEvent.UserId, userLoggedInEvent.Timestamp);
                }

                Console.WriteLine($"[Analytics] User {userLoggedInEvent.Email} logged in at {userLoggedInEvent.Timestamp}");
            };

            _channel.BasicConsume(queue: "user-logged-in",
                                 autoAck: true,
                                 consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
