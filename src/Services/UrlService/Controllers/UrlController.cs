using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using UrlService.Events;
using UrlService.Models.Requests;
using UrlService.Repositories;
using UrlService.Services;
using UrlService.Services.UrlShortening.Commands;
using UrlService.Services.UrlShortening.Queries;

namespace UrlService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlRepository _urlRepository;
        private readonly UrlShorteningService _urlShorteningService;
        private readonly IMediator _mediator;
        private readonly ClickEventPublisher _clickEventPublisher;

        public UrlController(IUrlRepository urlRepository, UrlShorteningService urlShorteningService, IMediator mediator, ClickEventPublisher clickEventPublisher)
        {
            _urlRepository = urlRepository;
            _urlShorteningService = urlShorteningService;
            _mediator = mediator;
            _clickEventPublisher = clickEventPublisher;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            string shortCode = await _mediator.Send(command);
            return Ok(new {shortCode});
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetOriginalUrl(string shortCode)
        {
            var originalUrl = await _mediator.Send(new GetOriginalUrlQuery(shortCode));

            if (originalUrl == null) return NotFound();

            /// track click event
            //var client = new HttpClient();
            //await client.PostAsJsonAsync("http://localhost:5004/api/analytics/track", new 
            //{
            //    ShortCode = shortCode,
            //    UserId = User.Identity?.Name,
            //    IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
            //    UserAgent = Request.Headers["User-Agent"].ToString(),
            //    CreatedAt = DateTimeOffset.Now
            //});

            _clickEventPublisher.PublishClickEvent(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, shortCode);

            return Redirect(originalUrl);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("my-urls")]
        public async Task<IActionResult> GetUserUrls()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var shortUrls = await _urlRepository.GetAllByUserId(Guid.Parse(userId));
            return Ok(shortUrls);
        }

        [HttpGet("count-by-tenant")]
        public async Task<IActionResult> GetTotalUrlByTenantId(string tenantId)
        {
            ///// Todo:
            return Ok(10);
        }
    }
}
