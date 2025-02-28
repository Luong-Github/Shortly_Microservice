using IdentityService.Data;
using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services
{
    public interface IAffiliateService
    {
        Task<string> RegisterAffiliate(Guid businessId, decimal commissionRate);
        Task<bool> RegisterReferral(Guid affiliateId, Guid referredUserId, decimal orderAmount);
        Task<decimal> GetAffiliateEarnings(Guid affiliateId);
        Task<bool> ProcessPayout(Guid affiliateId, string payoutMethod);
    }
    public class AffiliateService : IAffiliateService
    {
        private readonly AppIdentityDbContext _context;

        public AffiliateService(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterAffiliate(Guid businessId, decimal commissionRate)
        {
            var referralCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            var affiliate = new Affiliate
            {
                Id = Guid.NewGuid(),
                BusinessId = businessId,
                ReferralCode = referralCode,
                CommissionRate = commissionRate
            };

            _context.Set<Affiliate>().Add(affiliate);
            await _context.SaveChangesAsync();
            return referralCode;
        }

        public async Task<bool> RegisterReferral(Guid affiliateId, Guid referredUserId, decimal orderAmount)
        {
            var affiliate = await _context.Set<Affiliate>().FindAsync(affiliateId);
            if (affiliate == null) return false;

            decimal commissionEarned = (orderAmount * affiliate.CommissionRate) / 100;

            _context.Set<AffiliateReferral>().Add(new AffiliateReferral
            {
                Id = Guid.NewGuid(),
                AffiliateId = affiliateId,
                ReferredUserId = referredUserId,
                EarnedCommission = commissionEarned
            });

            affiliate.TotalEarnings += commissionEarned;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetAffiliateEarnings(Guid affiliateId)
        {
            var affiliate = await _context.Set<Affiliate>().FindAsync(affiliateId);
            return affiliate?.TotalEarnings ?? 0;
        }

        public async Task<bool> ProcessPayout(Guid affiliateId, string payoutMethod)
        {
            var affiliate = await _context.Set<Affiliate>().FindAsync(affiliateId);
            if (affiliate == null || affiliate.TotalEarnings == 0) return false;

            var payout = new AffiliatePayout
            {
                Id = Guid.NewGuid(),
                AffiliateId = affiliateId,
                Amount = affiliate.TotalEarnings,
                PayoutDate = DateTime.UtcNow,
                PayoutMethod = payoutMethod,
                IsProcessed = true
            };

            _context.Set<AffiliatePayout>().Add(payout);
            affiliate.TotalEarnings = 0; // Reset earnings after payout
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
