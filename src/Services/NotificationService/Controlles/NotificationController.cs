using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controlles
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(string userId)
        {
            var notifications = await _notificationService.GetNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(string userId, bool? isRead, DateTime? startDate, DateTime? endDate, string keyword)
        {
            var notifications = await _notificationService.GetNotificationsAsync(userId, isRead, startDate, endDate, keyword);
            return Ok(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(string userId, bool? isRead, DateTime? startDate, DateTime? endDate, string keyword, int page = 1, int pageSize = 10)
        {
            var notifications = await _notificationService.GetNotificationsAsync(userId, isRead, startDate, endDate, keyword, page, pageSize);
            return Ok(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification(Notification notification)
        {
            await _notificationService.AddNotificationAsync(notification);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }
        [HttpGet("analytics")]
        public async Task<IActionResult> GetNotificationAnalytics(DateTime startDate, DateTime endDate)
        {
            var analytics = await _notificationService.GetNotificationAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportAnalytics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format)
        {
            var data = await _notificationService.ExportAnalyticsAsync(startDate, endDate, format);

            string contentType = format switch
            {
                "csv" => "text/csv",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            return File(data, contentType, $"Analytics-{DateTime.UtcNow:yyyyMMdd}.{format}");
        }
    }
}
