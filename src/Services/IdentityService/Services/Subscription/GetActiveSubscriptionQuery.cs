using IdentityService.Data;
using IdentityService.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services.Subscription
{
    public class GetActiveSubscriptionQuery : IRequest<TenantSubscription>
    {
        public Guid TenantId { get; set; }

    }

    public class GetActiveSubscriptionQueryHandler : IRequestHandler<GetActiveSubscriptionQuery, TenantSubscription>
    {
        private readonly AppIdentityDbContext _context;

        public GetActiveSubscriptionQueryHandler(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<TenantSubscription> Handle(GetActiveSubscriptionQuery request, CancellationToken cancellationToken)
        {
            return await _context.TenantSubscriptions
           .FirstOrDefaultAsync(s => s.TenantId == request.TenantId && s.ExpiryDate > DateTime.UtcNow);
        }
    }
}
