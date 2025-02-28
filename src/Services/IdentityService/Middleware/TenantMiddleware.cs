using IdentityService.Services.Tenant;
using MediatR;

namespace IdentityService.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMediator _mediator;

        public TenantMiddleware(RequestDelegate next, IMediator mediator)
        {
            _next = next;
            _mediator = mediator;
        }

        public async Task Invoke(HttpContext context)
        {
            string host = context.Request.Host.Value;
            var tenant = await _mediator.Send(new GetTenantAsyncQuery() { Domain = host });

            if (tenant != null)
            {
                context.Items["Tenant"] = tenant;
            }

            await _next(context);
        }
    }
}
