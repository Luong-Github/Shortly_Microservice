using AnalyticsService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [Route("api/auth-analytics")]
    [ApiController]
    public class AuthAnalyticsController : ControllerBase
    {
        private readonly RedisService _redisService;

        public AuthAnalyticsController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("login-history/{userId}")]
        public async Task<IActionResult> GetUserLoginHistory(string userId, [FromQuery] int limit = 10)
        {
            if (userId == null) return BadRequest("User Id is required");

            var loginHistory = await _redisService.GetUserLoginHistoryAsync(userId, limit);
            if (loginHistory == null || loginHistory.Count == 0) return NotFound();
            return Ok( new { userId, loginHistory = loginHistory});
        }

    }
}
