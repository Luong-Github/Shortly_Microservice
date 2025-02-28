using Shared.Domain.Abstractions;
using Shared.Domain.Abstractions.IEntites;

namespace NotificationService.Models
{
    public class AdminAlert : IDateTracking
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
        public bool SendSlack { get; set; }
        public bool SendTelegram { get; set; }
        public bool SendPushNotification { get; set; }
        public string DeviceToken { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}
