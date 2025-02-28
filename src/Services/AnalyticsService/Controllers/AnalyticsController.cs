using AnalyticsService.ClickAnalytics.Commands;
using AnalyticsService.ClickAnalytics.Queries;
using AnalyticsService.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly IMediator _mediator;
        
        public AnalyticsController(IAnalyticsRepository analyticsRepository, IMediator mediator)
        {
            _analyticsRepository = analyticsRepository;
            _mediator = mediator;
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackClickAsync([FromBody] LogClickCommand command)
        {
            await _mediator.Send(command);
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
            var totalClicks = await _mediator.Send(new GetTotalClickQuery(shortCode));
            return Ok(new {shortCode, totalClicks = totalClicks});
        }
    }
}
