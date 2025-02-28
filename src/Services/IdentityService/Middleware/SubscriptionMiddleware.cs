using IdentityService.Entities;
using IdentityService.Services.Subscription;
using MediatR;
using Stripe;

namespace IdentityService.Middleware
{
    public class SubscriptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMediator _mediator;

        public SubscriptionMiddleware(RequestDelegate next, IMediator mediator)
        {
            _next = next;
            _mediator = mediator;
        }

        public async Task Invoke(HttpContext context)
        {
            var tenant = context.Items["Tenant"] as Tenant;
            if (tenant != null)
            {
                var subscription = await _mediator.Send(new GetActiveSubscriptionQuery() { TenantId = tenant.Id});
                if (subscription == null || subscription.PaymentStatus != "Paid")
                {
                    context.Response.StatusCode = 402; // Payment Required
                    await context.Response.WriteAsync("Subscription expired or missing!");
                    return;
                }
            }

            await _next(context);
        }
    }
}
