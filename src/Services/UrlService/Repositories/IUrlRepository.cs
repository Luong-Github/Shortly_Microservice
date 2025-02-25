using UrlService.Models;

namespace UrlService.Repositories
{
    public interface IUrlRepository
    {
        Task<ShortUrl> GetByShortCodeAsync(string shortCode);
        Task<ShortUrl> CreateAsync(ShortUrl shortUrl);
        Task<List<ShortUrl>> GetAllByUserId(Guid userId);
    }
}
