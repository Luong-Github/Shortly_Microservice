using IdentityService.Data;
using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Jobs
{
    public class ExpiredReferralCleanupJob : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ExpiredReferralCleanupJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var expiredRewards = await context.ReferralRewards
                    .Where(r => r.ExpiryDate < DateTime.UtcNow && !r.IsUsed).ToListAsync();

                context.ReferralRewards.RemoveRange(expiredRewards);
                await context.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
