using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlService.Models.Requests;
using UrlService.Repositories;
using UrlService.Services;

namespace UrlService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlRepository _urlRepository;
        private readonly UrlShorteningService _urlShorteningService;

        public UrlController(IUrlRepository urlRepository, UrlShorteningService urlShorteningService)
        {
            _urlRepository = urlRepository;
            _urlShorteningService = urlShorteningService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] UrlRequest model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var shortUrl = await _urlShorteningService.CreateShortenUrlAsync(model.OriginalUrl, Guid.Parse(userId));
            return Ok(new {shortUrl.ShortCode});
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetOriginalUrl(string shortCode)
        {
            var shortUrl = await _urlRepository.GetByShortCodeAsync(shortCode);
            if (shortUrl == null)
            {
                return NotFound();
            }

            /// track click event
            var client = new HttpClient();
            await client.PostAsJsonAsync("http://localhost:5004/api/analytics/track", new 
            {
                ShortCode = shortCode,
                UserId = User.Identity?.Name,
                IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                CreatedAt = DateTimeOffset.Now
            });

            return Redirect(shortUrl.OriginalUrl);
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
    }
}
