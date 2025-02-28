using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class TenantSubscription : EntityAuditBase<Guid>
    {
        public Guid TenantId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string PaymentStatus { get; set; } // Paid, Pending, Expired
    }
}
