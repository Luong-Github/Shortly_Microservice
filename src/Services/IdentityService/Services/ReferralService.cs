using IdentityService.Data;
using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services
{
    public interface IReferralService
    {
        Task<string> GenerateReferralCode(Guid userId);
        Task<bool> RegisterReferral(Guid referrerId, Guid referredUserId);
        Task<decimal> GetReferralDiscount(Guid userId);
    }
    public class ReferralService : IReferralService
    {
        private readonly AppIdentityDbContext _context;

        public ReferralService(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateReferralCode(Guid userId)
        {
            var referralCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            _context.Set<Referral>().Add(new Referral
            {
                Id = Guid.NewGuid(),
                ReferrerId = userId,
                ReferralCode = referralCode
            });

            await _context.SaveChangesAsync();
            return referralCode;
        }

        public async Task<bool> RegisterReferral(Guid referrerId, Guid referredUserId)
        {
            var referral = await _context.Referrals.FirstOrDefaultAsync(r => r.ReferrerId == referrerId && r.ReferredUserId == null);
            if (referral == null) return false;

            referral.ReferredUserId = referredUserId;
            referral.IsRewarded = true;

            // Grant discount to both users
            _context.Set<ReferralReward>().AddRange(new[]
            {
            new ReferralReward { Id = Guid.NewGuid(), UserId = referrerId, DiscountAmount = 10, ExpiryDate = DateTime.UtcNow.AddMonths(1) },
            new ReferralReward { Id = Guid.NewGuid(), UserId = referredUserId, DiscountAmount = 5, ExpiryDate = DateTime.UtcNow.AddMonths(1) }
        });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetReferralDiscount(Guid userId)
        {
            var reward = await _context.ReferralRewards.FirstOrDefaultAsync(r => r.UserId == userId && !r.IsUsed && r.ExpiryDate > DateTime.UtcNow);
            return reward?.DiscountAmount ?? 0;
        }
    }
}
