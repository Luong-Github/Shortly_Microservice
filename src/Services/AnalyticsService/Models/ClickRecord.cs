using Shared.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AnalyticsService.Models
{
    public class ClickRecord : EntityAuditBase<int>
    {
        [Required]
        public string ShortCode { get; set; } = string.Empty;

        public string? UserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
