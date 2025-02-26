using AnalyticsService.Entities;

namespace AnalyticsService.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddLogAsync(AuditLog log);
        Task<IEnumerable<AuditLog>> GetAllAsync();
    }
}
