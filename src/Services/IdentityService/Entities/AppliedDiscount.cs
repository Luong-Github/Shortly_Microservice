using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class AppliedDiscount : EntityAuditBase<Guid>
    {
        public Guid TenantId { get; set; }
        public Guid DiscountCodeId { get; set; }
        public DateTime AppliedDate { get; set; }
    }
}
