using Shared.Domain.Abstractions;

namespace NotificationService.Models
{
    public class Notification : EntityAuditBase<int>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string UserId { get; set; }
    }
}
