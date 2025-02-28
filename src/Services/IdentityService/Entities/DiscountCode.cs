using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class DiscountCode : EntityAuditBase<Guid>
    {
        public string Code { get; set; } // e.g., "NEWYEAR2025"
        public decimal DiscountAmount { get; set; } // Fixed amount discount
        public decimal DiscountPercentage { get; set; } // Percentage discount
        public DateTime ExpiryDate { get; set; } // Expiration date
        public int MaxRedemptions { get; set; } // Limit usage
        public int RedemptionCount { get; set; } // Track usage
        public bool IsActive { get; set; }
    }
}
