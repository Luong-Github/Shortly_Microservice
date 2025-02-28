using IdentityService.Data;
using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Jobs
{
    public class ExpiredDiscountCleanupJob : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ExpiredDiscountCleanupJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var expiredCoupons = await context.DiscountCodes.Where(d => d.ExpiryDate < DateTime.UtcNow && d.IsActive).ToListAsync();
                expiredCoupons.ForEach(d => d.IsActive = false);

                await context.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromHours(24), cancellationToken); // Runs daily
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
