using IdentityService.Data;
using IdentityService.Entities;
using IdentityService.Services;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Jobs
{
    public class AffiliatePayoutJob : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AffiliatePayoutJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var affiliateService = scope.ServiceProvider.GetRequiredService<IAffiliateService>();
            var _context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var affiliates = await _context.Affiliates.Where(a => a.TotalEarnings > 0).ToListAsync();
                foreach (var affiliate in affiliates)
                {
                    await affiliateService.ProcessPayout(affiliate.Id, "Bank Transfer");
                }
                await Task.Delay(TimeSpan.FromDays(30), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
