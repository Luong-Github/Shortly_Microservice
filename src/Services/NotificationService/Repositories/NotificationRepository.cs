using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotificationService.Dto;
using NotificationService.Models;
using RabbitMQ.Client;
using StackExchange.Redis;
using Twilio.TwiML.Messaging;

namespace NotificationService.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IConnection _connection;
        private readonly IDatabase _redisDb;
        private readonly string _redisKey = "notifications";


        public NotificationRepository(IConfiguration configuration) {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
            _redisDb = redis.GetDatabase();
        }
        public async Task<NotificationAnalyticsDto> GetGetAnalyticsAsync(DateTime startDate, DateTime endDate)
        {
            var data = await _redisDb.StringGetAsync(_redisKey);
            if (string.IsNullOrEmpty(data)) return new NotificationAnalyticsDto();

            var notifications = JsonConvert.DeserializeObject<List<Notification>>(data)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .ToList();

            var total = notifications.Count;
            var unread = notifications.Count(n => !n.IsRead);
            var read = notifications.Count(n => n.IsRead);
            var dailyStats = notifications.GroupBy(n => n.CreatedDate.Date)
                                          .Select(g => new DailyStatDto { Date = g.Key, Count = g.Count() })
                                          .ToList();

            return new NotificationAnalyticsDto
            {
                TotalNotifications = total,
                UnreadNotifications = unread,
                ReadNotifications = read,
                DailyStats = dailyStats
            };
        }
    }
}
