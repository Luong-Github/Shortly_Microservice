using MimeKit;
using NotificationService.Services;
using Quartz;
using MailKit.Net.Smtp;

namespace NotificationService.Jobs
{
    public class AnalyticsReportJob : IJob
    {
        private readonly INotificationService _notificationService;

        public AnalyticsReportJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("📩 Sending scheduled email report...");

            DateTime startDate = DateTime.UtcNow.AddDays(-7);
            DateTime endDate = DateTime.UtcNow;
            string format = "pdf"; // Default to PDF, can be changed

            byte[] reportData = await _notificationService.ExportAnalyticsAsync(startDate, endDate, format);
            await SendEmailWithAttachment("admin@example.com", reportData, format);
        }

        private async Task SendEmailWithAttachment(string toEmail, byte[] attachmentData, string format)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Analytics Service", "noreply@example.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "📊 Weekly Analytics Report";

            var body = new TextPart("plain")
            {
                Text = "Hello Admin,\n\nPlease find the attached analytics report.\n\nBest regards,\nAnalytics Service"
            };

            var attachment = new MimePart("application", format == "pdf" ? "pdf" : "octet-stream")
            {
                Content = new MimeContent(new MemoryStream(attachmentData)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = $"AnalyticsReport-{DateTime.UtcNow:yyyyMMdd}.{format}"
            };

            var multipart = new Multipart("mixed") { body, attachment };
            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.example.com", 587, false);
            await client.AuthenticateAsync("your-email@example.com", "your-password");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            Console.WriteLine("✅ Email sent successfully!");
        }

    }
}
