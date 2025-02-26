using AnalyticsService.Entities;
using AnalyticsService.Repositories;
using System.Security.Claims;

namespace AnalyticsService.Middlewares
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuditLogRepository auditLogRepository)
        {
            if (context.User.Identity.IsAuthenticated && context.User.IsInRole("Admin"))
            {
                var log = new AuditLog
                {
                    AdminUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    AdminUserName = context.User.Identity.Name,
                    Action = $"{context.Request.Method} {context.Request.Path}",
                    Endpoint = context.Request.Path,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString()
                };

                await auditLogRepository.AddLogAsync(log);
            }

            await _next(context);
        }
    }
}
