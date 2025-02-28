using NotificationService.Dto;

namespace NotificationService.Repositories
{
    public interface INotificationRepository
    {
        Task<NotificationAnalyticsDto> GetGetAnalyticsAsync(DateTime startDate, DateTime endDate);
    }
}
