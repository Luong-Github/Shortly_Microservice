using IdentityService.Data;
using IdentityService.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services.Discount
{
    public class ApplyDiscountCommand : IRequest<bool>
    {
       public Guid TenantId { get; set; }
       public string Code { get; set; }
    }

    public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, bool>
    {
        private readonly AppIdentityDbContext _context;

        public ApplyDiscountCommandHandler(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _context.DiscountCodes.FirstOrDefaultAsync(d => d.Code == request.Code && d.IsActive);
            if (discount == null || discount.RedemptionCount >= discount.MaxRedemptions || discount.ExpiryDate < DateTime.UtcNow)
                return false; // Invalid or expired discount

            _context.Set<AppliedDiscount>().Add(new AppliedDiscount
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                DiscountCodeId = discount.Id,
                AppliedDate = DateTime.UtcNow
            });

            discount.RedemptionCount += 1;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
