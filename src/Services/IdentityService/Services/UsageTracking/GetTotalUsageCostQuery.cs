using IdentityService.Data;
using IdentityService.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services.UsageTracking
{
    public class GetTotalUsageCostQuery : IRequest<decimal>
    {
        public Guid tenantId { get; set; }
    }

    public class GetTotalUsageCostQueryHandler : IRequestHandler<GetTotalUsageCostQuery, decimal>
    {
        private readonly AppIdentityDbContext _context;

        public GetTotalUsageCostQueryHandler(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task<decimal> Handle(GetTotalUsageCostQuery request, CancellationToken cancellationToken)
        {
            return await _context.UsageRecords
            .Where(u => u.TenantId == request.tenantId)
            .SumAsync(u => u.Quantity * u.CostPerUnit);
        }
    }
}
