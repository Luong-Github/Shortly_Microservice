using AnalyticsService.Models;

namespace AnalyticsService.Repositories
{
    public interface IAnalyticsRepository
    {
        Task LogClickAsync(ClickRecord clickRecord);
        Task<int> GetTotalClickAsync(string shortCode);
        Task<List<ClickRecord>> GetClickRecordsByUserIdAsync(string userId);
        Task<int> GetTotalClicksFromCacheAsync(string shortCode);
    }
}
