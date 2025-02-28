using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class TenantBilling : EntityAuditBase<Guid>
    {
        public Guid TenantId { get; set; }
        public decimal TotalAmountDue { get; set; }
        public bool IsPaid { get; set; }
        public DateTime BillingCycleStart { get; set; }
        public DateTime BillingCycleEnd { get; set; }
    }
}
