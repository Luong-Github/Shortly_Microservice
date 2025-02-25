using Microsoft.EntityFrameworkCore;
using UrlService.Data;
using UrlService.Models;

namespace UrlService.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly UrlDbContext _context;

        public UrlRepository(UrlDbContext context)
        {
            _context = context;
        }

        public async Task<ShortUrl> CreateAsync(ShortUrl shortUrl)
        {
            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();
            return shortUrl;
        }

        public Task<List<ShortUrl>> GetAllByUserId(Guid userId)
        {
            return _context.ShortUrls.Where(u => u.CreatedBy == userId).ToListAsync();
        }

        public async Task<ShortUrl> GetByShortCodeAsync(string shortCode) 
            => await _context.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    }
}
