using MediatR;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;
using IdentityService.Events;
using System.Text.Json;
using System.Text;

namespace IdentityService.Events
{
    public class UserLoggedInEvent : INotification
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public UserLoggedInEvent(string userId, string email, string token)
        {
            UserId = userId;
            Email = email;
            Token = token;
        }
    }

    public class UserLoggedInEventHandler : INotificationHandler<UserLoggedInEvent>
    {
        public Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "user-logged-in",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            var message = JsonSerializer.Serialize(notification);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "user-logged-in",
                                 basicProperties: null,
                                 body: body);

            // send email
            return Task.CompletedTask;
        }
    }
}
