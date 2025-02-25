using AnalyticsService.Data;
using AnalyticsService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace AnalyticsService.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppAnalyticsDbContext _dbContext;
        private readonly IDatabase _redisDb;

        private const string ClickRedisPrefix = "clicks:";
        public AnalyticsRepository(AppAnalyticsDbContext _dbContext, IConnectionMultiplexer redis)
        {
            this._dbContext = _dbContext;
            this._redisDb = redis.GetDatabase();
        }
        public async Task<List<ClickRecord>> GetClickRecordsByUserIdAsync(string userId)
        {
            return await _dbContext.ClickRecords.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<int> GetTotalClickAsync(string shortCode)
        {
            return await _dbContext.ClickRecords.CountAsync(c => c.ShortCode == shortCode);
        }

        public async Task LogClickAsync(ClickRecord clickRecord)
        {
            _dbContext.ClickRecords.Add(clickRecord);
            await _dbContext.SaveChangesAsync();
            
            // Increment Redis clicks count
            await _redisDb.StringIncrementAsync(ClickRedisPrefix + clickRecord.ShortCode);
        }

        public async Task<int> GetTotalClicksFromCacheAsync(string shortCode)
        {
            var cacheClicks = await _redisDb.StringGetAsync(ClickRedisPrefix + shortCode);
            if(cacheClicks.HasValue)
            {
                return (int)cacheClicks;
            }

            int totalClicks = await GetTotalClickAsync(shortCode);
            await _redisDb.StringSetAsync(ClickRedisPrefix + shortCode, totalClicks, TimeSpan.FromMinutes(5));
            return totalClicks;
        }
    }
}
