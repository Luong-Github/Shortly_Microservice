using IdentityService.Data;
using IdentityService.Entities;
using IdentityService.Services.Discount;
using IdentityService.Services.UsageTracking;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace IdentityService.Services
{
    public interface IBillingService
    {
        Task ProcessBilling(Guid tenantId);
        Task GenerateMonthlyInvoices();
    }
    public class BillingService : IBillingService
    {
        private readonly AppIdentityDbContext _context;
        private readonly IMediator _mediator;
        private readonly IReferralService _referralService;
        public BillingService(IMediator mediator, AppIdentityDbContext context, IReferralService referralService)
        {
            _mediator = mediator;
            _context = context;
            _referralService = referralService;
        }

        public async Task ProcessBilling(Guid tenantId)
        {
            decimal totalAmount = await _mediator.Send(new GetTotalUsageCostQuery() { tenantId = tenantId });
            decimal referralDiscount = await _referralService.GetReferralDiscount(tenantId);
            decimal discount = await _mediator.Send(new GetDiscountAmountQuery()
            {
                TenantId = tenantId,
                TotalAmount = totalAmount - referralDiscount
            });
            decimal finalAmount = Math.Max(0, totalAmount - discount); // Ensure no negative amounts

            var paymentSuccess = await StripePaymentService.ProcessPayment(tenantId.ToString(), finalAmount);
            if (paymentSuccess)
            {
                var reward = await _context.ReferralRewards.FirstOrDefaultAsync(r => r.UserId == tenantId && !r.IsUsed);
                if (reward != null) reward.IsUsed = true;

                _context.TenantBilling.Add(new TenantBilling
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    TotalAmountDue = finalAmount,
                    IsPaid = true,
                    BillingCycleStart = DateTime.UtcNow.AddMonths(-1),
                    BillingCycleEnd = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task GenerateMonthlyInvoices()
        {
            var tenants = await _context.Tenants.ToListAsync();

            foreach (var tenant in tenants)
            {
                decimal totalUsageCost = await _mediator.Send(new GetTotalUsageCostQuery() { tenantId = tenant.Id });
                if (totalUsageCost > 0)
                {
                    // Process Payment
                    var paymentSuccess = await StripePaymentService.ProcessPayment(tenant.StripeCustomerId.ToString(), totalUsageCost);
                    if (paymentSuccess)
                    {
                        // Mark as Paid
                        var billing = new TenantBilling
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenant.Id,
                            TotalAmountDue = totalUsageCost,
                            IsPaid = true,
                            BillingCycleStart = DateTime.UtcNow.AddMonths(-1),
                            BillingCycleEnd = DateTime.UtcNow
                        };

                        _context.Set<TenantBilling>().Add(billing);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
