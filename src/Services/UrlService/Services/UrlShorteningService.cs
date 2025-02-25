using System.Security.Cryptography;
using System.Text;
using UrlService.Models;
using UrlService.Repositories;

namespace UrlService.Services
{
    public class UrlShorteningService
    {
        private readonly IUrlRepository _urlRepository;

        public UrlShorteningService(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public async Task<ShortUrl> CreateShortenUrlAsync(string originalUrl, Guid userId)
        {
            var shortCode = GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedBy = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                IsDeleted = false
            };

            return await _urlRepository.CreateAsync(shortUrl);
        }

        private string GenerateShortCode() {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
            return ConvertToBase62(hashBytes);
        }

        private static string ConvertToBase62(byte[] hash)
        {
            const string base62 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var sb = new StringBuilder();

            foreach(byte b in hash)
            {
                sb.Append(base62[b % base62.Length]);
            }
            return sb.ToString();
        }
    }
}
