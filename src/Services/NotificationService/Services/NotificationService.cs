using CsvHelper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using NotificationService.Dto;
using NotificationService.Models;
using OfficeOpenXml;
using StackExchange.Redis;
using System.Globalization;
using NotificationService.Repositories;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsAsync(string userId);
        Task<List<Notification>> GetNotificationsAsync(string userId, bool? isRead, DateTime? startDate, DateTime? endDate, string keyword);
        Task<List<Notification>> GetNotificationsAsync(string userId, bool? isRead, DateTime? startDate, DateTime? endDate, string keyword, int page, int pageSize);
        Task AddNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int notificationId);
        Task<NotificationAnalyticsDto> GetNotificationAnalyticsAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> ExportAnalyticsAsync(DateTime startDate, DateTime endDate, string format);

    }
    public class NotificationService : INotificationService
    {
        private readonly IDatabase _redisDb;
        private readonly string _redisKey = "notifications";
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(IConfiguration configuration, INotificationRepository notificationRepository)
        {
            _notificationRepo = notificationRepository;
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
            _redisDb = redis.GetDatabase();
        }
        public async Task<List<Notification>> GetNotificationsAsync(string userId)
        {
            var data = await _redisDb.StringGetAsync(_redisKey);
            if (string.IsNullOrEmpty(data)) return new List<Notification>();

            var notifications = JsonConvert.DeserializeObject<List<Notification>>(data);
            return notifications.Where(n => n.UserId == userId).ToList();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            var data = await _redisDb.StringGetAsync(_redisKey);
            var notifications = string.IsNullOrEmpty(data)
                ? new List<Notification>()
                : JsonConvert.DeserializeObject<List<Notification>>(data);

            notifications.Insert(0, notification); // Insert at the top
            await _redisDb.StringSetAsync(_redisKey, JsonConvert.SerializeObject(notifications));
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var data = await _redisDb.StringGetAsync(_redisKey);
            if (string.IsNullOrEmpty(data)) return;

            var notifications = JsonConvert.DeserializeObject<List<Notification>>(data);
            var notification = notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null) notification.IsRead = true;

            await _redisDb.StringSetAsync(_redisKey, JsonConvert.SerializeObject(notifications));
        }
        public async Task<List<Notification>> GetNotificationsAsync(string userId, bool? isRead, DateTime? startDate, DateTime? endDate, string keyword)
        {
            var data = await _redisDb.StringGetAsync(_redisKey);
            if (string.IsNullOrEmpty(data)) return new List<Notification>();

            var notifications = JsonConvert.DeserializeObject<List<Notification>>(data)
                .Where(n => n.UserId == userId)
                .ToList();

            if (isRead.HasValue)
                notifications = notifications.Where(n => n.IsRead == isRead.Value).ToList();

            if (startDate.HasValue && endDate.HasValue)
                notifications = notifications.Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate).ToList();

            if (!string.IsNullOrEmpty(keyword))
                notifications = notifications.Where(n => n.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                                         n.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            return notifications.OrderByDescending(n => n.CreatedDate).ToList();
        }

        public async Task<List<Notification>> GetNotificationsAsync(string userId, bool? isRead, DateTime? startDate, DateTime? endDate, string keyword, int page, int pageSize)
        {
            var data = await _redisDb.StringGetAsync(_redisKey);
            if (string.IsNullOrEmpty(data)) return new List<Notification>();

            var notifications = JsonConvert.DeserializeObject<List<Notification>>(data)
                .Where(n => n.UserId == userId)
                .ToList();

            if (isRead.HasValue)
                notifications = notifications.Where(n => n.IsRead == isRead.Value).ToList();

            if (startDate.HasValue && endDate.HasValue)
                notifications = notifications.Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate).ToList();

            if (!string.IsNullOrEmpty(keyword))
                notifications = notifications.Where(n => n.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                                         n.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            return notifications
                .OrderByDescending(n => n.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<NotificationAnalyticsDto> GetNotificationAnalyticsAsync(DateTime startDate, DateTime endDate)
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
                                          .Select(g => new DailyStatDto{ Date = g.Key, Count = g.Count() })
                                          .ToList();

            return new NotificationAnalyticsDto
            {
                TotalNotifications = total,
                UnreadNotifications = unread,
                ReadNotifications = read,
                DailyStats = dailyStats
            };
        }

        public async Task<byte[]> ExportAnalyticsAsync(DateTime startDate, DateTime endDate, string format)
        {
            var analytics = await _notificationRepo.GetGetAnalyticsAsync(startDate, endDate);

            return format.ToLower() switch
            {
                "csv" => GenerateCsv(analytics),
                "excel" => GenerateExcel(analytics),
                "pdf" => GeneratePdf(analytics),
                _ => throw new ArgumentException("Invalid format")
            };
        }

        private byte[] GenerateCsv(NotificationAnalyticsDto analytics)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(new[]
            {
            new { analytics.TotalNotifications, analytics.ReadNotifications, analytics.UnreadNotifications }
        });

            writer.Flush();
            return stream.ToArray();
        }

        private byte[] GenerateExcel(NotificationAnalyticsDto analytics)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Analytics");

            worksheet.Cells[1, 1].Value = "Total Notifications";
            worksheet.Cells[1, 2].Value = "Read";
            worksheet.Cells[1, 3].Value = "Unread";
            worksheet.Cells[2, 1].Value = analytics.TotalNotifications;
            worksheet.Cells[2, 2].Value = analytics.ReadNotifications;
            worksheet.Cells[2, 3].Value = analytics.UnreadNotifications;

            return package.GetAsByteArray();
        }

        private byte[] GeneratePdf(NotificationAnalyticsDto analytics)
        {
            using var stream = new MemoryStream();
            var document = new Document();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            document.Add(new Paragraph($"Total: {analytics.TotalNotifications}"));
            document.Add(new Paragraph($"Read: {analytics.ReadNotifications}"));
            document.Add(new Paragraph($"Unread: {analytics.UnreadNotifications}"));

            document.Close();
            return stream.ToArray();
        }
    }
}
