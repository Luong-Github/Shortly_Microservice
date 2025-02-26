namespace ApiGateway.Middlewares
{
    public class RateLimitLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitLoggingMiddleware> _logger;

        public RateLimitLoggingMiddleware(RequestDelegate next, ILogger<RateLimitLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Response.StatusCode == 429)
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();
                var endpoint = context.Request.Path;
                _logger.LogWarning($"Rate limit exceeded: IP={ip}, Endpoint={endpoint}");
            }

            await _next(context);
        }
    }
}
