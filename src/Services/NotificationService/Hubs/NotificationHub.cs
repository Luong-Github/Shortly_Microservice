using Microsoft.AspNetCore.SignalR;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task SendNotification(string userId, string title, string message)
        {
            var notification = new Notification { Title = title, Message = message, UserId = userId };
            await _notificationService.AddNotificationAsync(notification);
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }

        public async Task SendNotificationAnalyticsUpdate()
        {
            var analytics = await _notificationService.GetNotificationAnalyticsAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
            await Clients.All.SendAsync("ReceiveAnalyticsUpdate", analytics);
        }
    }
}
