using StackExchange.Redis;
using System.Text.Json;

namespace AnalyticsService.Services
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
            _db = _redis.GetDatabase();
        }

        public async Task TrackUserLoginAsync(string userId, DateTime timestamp)
        {
            string key = $"user:login:{userId}";
            await _db.SortedSetAddAsync(key, timestamp.ToString("o"), timestamp.Ticks);
        }

        public async Task<List<DateTime>> GetUserLoginHistoryAsync(string userId, int limit = 10)
        {
            string key = $"user:login:{userId}";
            var loginTimestamps = await _db.SortedSetRangeByRankAsync(key, 0, limit - 1, Order.Descending);
            return loginTimestamps.Select(ts => DateTime.Parse(ts.ToString())).ToList();
        }
    }
}
