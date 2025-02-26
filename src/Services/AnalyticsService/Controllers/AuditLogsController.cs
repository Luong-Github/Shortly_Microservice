using AnalyticsService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [Route("api/audit-logs")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogsController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _auditLogRepository.GetAllAsync();
            return Ok(logs);
        }
    }
}
