using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class AffiliateReferral : EntityAuditBase<Guid>
    {
        public Guid AffiliateId { get; set; }
        public Guid ReferredUserId { get; set; }
        public decimal EarnedCommission { get; set; } // Commission earned
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
