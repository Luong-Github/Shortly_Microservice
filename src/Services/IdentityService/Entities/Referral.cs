using Shared.Domain.Abstractions;

namespace IdentityService.Entities
{
    public class Referral : EntityAuditBase<Guid>
    {
        public Guid ReferrerId { get; set; } // The user who shared the link
        public Guid? ReferredUserId { get; set; } // The user who signed up
        public string ReferralCode { get; set; } // Unique referral code
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRewarded { get; set; } = false; // If rewards are given
    }
}
