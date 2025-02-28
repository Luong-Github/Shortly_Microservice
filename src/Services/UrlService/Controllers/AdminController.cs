using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlService.Data;

namespace UrlService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UrlDbContext _dbContext;

        public AdminController(UrlDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Get expired URLs
        [HttpGet("expired-urls")]
        public async Task<IActionResult> GetExpiredUrls()
        {
            var expiredUrls = await _dbContext.ShortUrls
                .Where(u => u.ExpirationDate < DateTime.UtcNow)
                .ToListAsync();
            return Ok(expiredUrls);
        }

        // Delete an expired URL
        [HttpDelete("expired-urls/{id}")]
        public async Task<IActionResult> DeleteExpiredUrl(Guid id)
        {
            var url = await _dbContext.ShortUrls.FindAsync(id);
            if (url == null) return NotFound();

            _dbContext.ShortUrls.Remove(url);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "URL deleted successfully" });
        }
    }
}
