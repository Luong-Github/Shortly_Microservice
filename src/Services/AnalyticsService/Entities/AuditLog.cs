namespace AnalyticsService.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public string Action { get; set; }
        public string Endpoint { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
