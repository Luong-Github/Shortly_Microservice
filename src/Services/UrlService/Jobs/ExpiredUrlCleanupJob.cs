using Microsoft.EntityFrameworkCore;
using Quartz;
using UrlService.Data;

namespace UrlService.Jobs
{
    public class ExpiredUrlCleanupJob : IJob
    {
        private readonly UrlDbContext _dbContext;

        public ExpiredUrlCleanupJob(UrlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var expiredUrls = await _dbContext.ShortUrls.Where(url => url.ExpirationDate < now).ToListAsync();

            if (expiredUrls.Any())
            {
                _dbContext.ShortUrls.RemoveRange(expiredUrls);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"[Expired URLs Cleanup] Deleted {expiredUrls.Count} expired URLs.");
            }
        }
    }
}
