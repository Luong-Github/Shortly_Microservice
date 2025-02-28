using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class UsageRecord : EntityAuditBase<Guid>
    {
        public Guid TenantId { get; set; }
        public string Feature { get; set; } // e.g., "ExtraURLs", "APIRequests"
        public int Quantity { get; set; } // Number of times used
        public decimal CostPerUnit { get; set; } // Price per unit
        public DateTime Timestamp { get; set; }
    }
}
