using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient
            {
                Host = _configuration["Email:SMTPHost"],
                Port = int.Parse(_configuration["Email:SMTPPort"]),
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                ),
                EnableSsl = true
            };
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Sender"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            return _smtpClient.SendMailAsync(mailMessage);
        }
        public async Task SendInvoiceEmail(string userEmail, decimal amount)
        {
            var client = new SmtpClient("smtp.example.com")
            {
                Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress("billing@example.com"),
                Subject = "Your Invoice",
                Body = $"Thank you for your payment of ${amount}. Your subscription is now active!",
                IsBodyHtml = true
            };

            message.To.Add(userEmail);
            await client.SendMailAsync(message);
        }

        public async Task SendOverageWarning(Guid tenantId, string feature, object? extraObject)
        {
            /// todo
            var tenant = extraObject;
            var email = "admin_email";

            var message = new MailMessage
            {
                From = new MailAddress("alerts@example.com"),
                Subject = "Usage Warning!",
                Body = $"You are reaching your limit for {feature}. Additional usage will be charged.",
                IsBodyHtml = true
            };

            message.To.Add(email);
            await new SmtpClient("smtp.example.com").SendMailAsync(message);
        }
    }
}
