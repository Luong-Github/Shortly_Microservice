using IdentityService.Data;
using IdentityService.Entities;
using MediatR;
using Stripe.Entitlements;

namespace IdentityService.Services.UsageTracking
{
    public class RecordUsageCommand : IRequest
    {
        public Guid TenantId { get; set; }
        public string Feature { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
    }

    public class RecordUsageCommandHandler : IRequestHandler<RecordUsageCommand>
    {
        private readonly AppIdentityDbContext _context;

        public RecordUsageCommandHandler(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task Handle(RecordUsageCommand request, CancellationToken cancellationToken)
        {
            var usageRecord = new UsageRecord
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                Feature = request.Feature,
                Quantity = request.Quantity,
                CostPerUnit = request.CostPerUnit,
                Timestamp = DateTime.UtcNow
            };

            _context.UsageRecords.Add(usageRecord);
            await _context.SaveChangesAsync();
        }
    }
}
