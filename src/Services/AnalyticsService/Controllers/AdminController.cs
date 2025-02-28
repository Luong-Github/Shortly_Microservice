using AnalyticsService.Data;
using AnalyticsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace AnalyticsService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppAnalyticsDbContext _dbContext;
        private readonly IConnectionMultiplexer _redis;

        public AdminController(AppAnalyticsDbContext dbContext, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _redis = redis;
        }
        // Get click analytics from SQL
        [HttpGet("analytics")]
        public async Task<List<ClickRecord>> GetAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _dbContext.ClickRecords
                .AsQueryable();

            if (startDate.HasValue) query = query.Where(a => a.CreatedDate >= startDate.Value);
            if (endDate.HasValue) query = query.Where(a => a.CreatedDate <= endDate.Value);

            var analyticsData = await query.OrderByDescending(a => a.CreatedDate).ToListAsync();
            return analyticsData;
        }

        // Clear all analytics from Redis
        [HttpDelete("analytics/clear-redis")]
        public async Task<string> ClearRedisAnalytics()
        {
            var db = _redis.GetDatabase();
            var redisKeys = _redis.GetServer("localhost", 6379).Keys(pattern: "analytics:*").ToList();

            foreach (var key in redisKeys)
            {
                await db.KeyDeleteAsync(key);
            }

            return "Redis analytics cleared successfully";
        }
    }
}
