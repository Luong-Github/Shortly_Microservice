using IdentityService.Data;
using IdentityService.Entities;
using MediatR;
using System.Numerics;
using System.Reflection.Metadata;
using Stripe;


namespace IdentityService.Services.Subscription
{
    public class ProcessPaymentCommand : IRequest<bool>
    {
        public Guid TenantId { get; set; }
        public Guid PlanId { get; set; }
        public string PaymentToken { get; set; }
    }

    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, bool>
    {
        private readonly AppIdentityDbContext _context;
        private readonly IMediator _mediator;

        public ProcessPaymentCommandHandler(AppIdentityDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            var plan = await _context.Set<SubscriptionPlan>().FindAsync(request.PlanId);
            if (plan == null) return false;

            // 🔥 Call Stripe API for payment processing
            var paymentSuccess = await StripePaymentService.ProcessPayment(request.PaymentToken, plan.Price);
            if (!paymentSuccess) return false;

            // Update Subscription
            var newSubscription = new TenantSubscription
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                PlanId = request.PlanId,
                StartDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
                PaymentStatus = "Paid"
            };

            _context.Set<TenantSubscription>().Add(newSubscription);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
