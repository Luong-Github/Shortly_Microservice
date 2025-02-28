
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotificationService.Hubs;
using NotificationService.Interfaces;
using NotificationService.Models;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Serialization;

namespace NotificationService.BackgroundServices
{
    public class AdminNotificationConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IHubContext<AdminNotificationHub> _hubContext;
        private readonly IEmailSender _emailSender;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly ISlackService _slackService;
        private readonly ITelegramService _telegramService;
        private readonly IConnection _rabbitMqConnection;
        private readonly IPushNotificationService _pushNotificationService;


        public AdminNotificationConsumer(IConnection connection, 
            IHubContext<AdminNotificationHub> hubContext, 
            IEmailSender sender, IEmailService emailService,
            ISmsService smsService,
            ISlackService slackService,
            ITelegramService telegramService,
            IPushNotificationService pushNotificationService)
        {
            _connection = connection;
            _hubContext = hubContext;
            _emailSender = sender;
            _emailService = emailService;
            _smsService = smsService;
            _slackService = slackService;
            _telegramService = telegramService;
            _pushNotificationService = pushNotificationService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "admin_alerts", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var alert = JsonConvert.DeserializeObject<AdminAlert>(message);

                if (alert.SendEmail)
                {
                    await _emailSender.SendEmailAsync("admin@example.com",
                        $"[ALERT] {alert.Type}",
                        $"<h3>{alert.Message}</h3><p>Time: {alert.CreatedDate}");
                }

                if (alert.SendSms)
                {
                    await _smsService.SendSmsAsync("+1234567890", alert.Message);
                }

                if (alert.SendSlack)
                {
                    await _slackService.SendSlackMessageAsync(alert.Message);
                }

                if (alert.SendTelegram)
                {
                    await _telegramService.SendTelegramMessageAsync(alert.Message);
                }

                if (alert.SendPushNotification)
                {
                    await _pushNotificationService.SendPushNotificationAsync(alert.DeviceToken, alert.Type, alert.Message);
                }

                await _hubContext.Clients.All.SendAsync("ReceiveAdminAlert", alert);
            };

            channel.BasicConsume(queue: "admin_alerts", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
