using AnalyticsService.Data;
using AnalyticsService.Entities;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace AnalyticsService.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppAnalyticsDbContext _context;
        private readonly IElasticClient _elasticClient;

        public AuditLogRepository(AppAnalyticsDbContext context, IElasticClient elasticClient)
        {
            _context = context;

            _elasticClient = elasticClient;
        }

        public async Task AddLogAsync(AuditLog log)
        {
            await _context.AuditLog.AddAsync(log);
            await _context.SaveChangesAsync();
            await _elasticClient.IndexDocumentAsync(log); // Send to Elasticsearch
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLog.Where(x => x != null).ToListAsync();
        }
    }
}
