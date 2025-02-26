using System.Text.Json;

namespace AnalyticsService.Events
{
    public class UserLoggerInEvent
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public UserLoggerInEvent(string userId, string email, string token)
        {
            UserId = userId;
            Email = email;
            Token = token;
        }

        public static UserLoggerInEvent FromJson(string json)
        {
            return JsonSerializer.Deserialize<UserLoggerInEvent>(json);
        }
    }
}
