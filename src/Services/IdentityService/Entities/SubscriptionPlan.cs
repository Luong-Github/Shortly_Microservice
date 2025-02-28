using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class SubscriptionPlan : EntityAuditBase<Guid>
    {
        public string Name { get; set; } // Free, Standard, Premium
        public decimal Price { get; set; }
        public int UrlLimit { get; set; } // Max URLs per month
        public bool HasAnalytics { get; set; } // True for premium plans
        public bool HasPrioritySupport { get; set; } // True for premium
    }
}
