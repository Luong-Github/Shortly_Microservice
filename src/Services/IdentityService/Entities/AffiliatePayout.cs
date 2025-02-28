using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class AffiliatePayout : EntityAuditBase<Guid>
    {
        public Guid AffiliateId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PayoutDate { get; set; }
        public string PayoutMethod { get; set; } // "PayPal", "Stripe", etc.
        public bool IsProcessed { get; set; } = false;
    }
}
