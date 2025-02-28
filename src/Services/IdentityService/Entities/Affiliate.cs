using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class Affiliate : EntityAuditBase<Guid>
    {
        public Guid BusinessId { get; set; } // Business ID in the system
        public string ReferralCode { get; set; } // Unique affiliate code
        public decimal CommissionRate { get; set; } // % of referred revenue
        public decimal TotalEarnings { get; set; } = 0; // Total earnings
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
