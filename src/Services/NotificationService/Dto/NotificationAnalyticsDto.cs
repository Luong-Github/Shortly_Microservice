namespace NotificationService.Dto
{
    public class NotificationAnalyticsDto
    {
        public int TotalNotifications { get; set; }
        public int UnreadNotifications { get; set; }
        public int ReadNotifications { get; set; }
        public List<DailyStatDto> DailyStats { get; set; }
    }

    public class DailyStatDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
