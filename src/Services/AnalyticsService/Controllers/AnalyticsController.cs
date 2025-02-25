using AnalyticsService.Models;
using AnalyticsService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        
        public AnalyticsController(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackClickAsync([FromBody]ClickRecord clickRecord)
        {
            await _analyticsRepository.LogClickAsync(clickRecord);
            return Ok( new {message = "Click logged successfully"});
        }

        [HttpGet("total/{shortCode}")]
        public async Task<IActionResult> GetTotalClickAsync(string shortCode)
        {
            var totalClick = await _analyticsRepository.GetTotalClickAsync(shortCode);
            return Ok(new {totalClick});
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetClickRecordsByUserIdAsync(string userId)
        {
            var clickRecords = await _analyticsRepository.GetClickRecordsByUserIdAsync(userId);
            return Ok(clickRecords);
        }

        [HttpGet("cached-total/{shortCode}")]
        public async Task<IActionResult> GetTotalClicksFromCacheAsync(string shortCode)
        {
            var totalClicks = await _analyticsRepository.GetTotalClicksFromCacheAsync(shortCode);
            return Ok(new {shortCode, totalClicks = totalClicks});
        }
    }
}
