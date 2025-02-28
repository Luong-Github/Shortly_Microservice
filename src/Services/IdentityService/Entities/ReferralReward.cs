using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class ReferralReward : EntityAuditBase<Guid>
    {
        public Guid UserId { get; set; } // Who received the reward
        public decimal DiscountAmount { get; set; } // Fixed discount for billing
        public DateTime ExpiryDate { get; set; } // When the discount expires
        public bool IsUsed { get; set; } = false;
    }
}
