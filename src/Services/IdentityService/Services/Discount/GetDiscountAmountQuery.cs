using IdentityService.Data;
using IdentityService.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services.Discount
{
    public class GetDiscountAmountQuery : IRequest<decimal>
    {
        public Guid TenantId { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class GetDiscountAmountQueryHandler : IRequestHandler<GetDiscountAmountQuery, decimal>
    {
        private readonly AppIdentityDbContext _context;

        public GetDiscountAmountQueryHandler(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task<decimal> Handle(GetDiscountAmountQuery request, CancellationToken cancellationToken)
        {
            var appliedDiscount = await _context.AppliedDiscounts
                .Where(d => d.TenantId == request.TenantId)
                .OrderByDescending(d => d.AppliedDate)
                .FirstOrDefaultAsync();

            if (appliedDiscount == null) return 0;

            var discount = await _context.Set<DiscountCode>().FindAsync(appliedDiscount.DiscountCodeId);
            if (discount == null) return 0;

            return discount.DiscountPercentage > 0 ? (request.TotalAmount * (discount.DiscountPercentage / 100)) : discount.DiscountAmount;
        }
    }
}
