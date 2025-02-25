using Microsoft.EntityFrameworkCore;
using UrlService.Models;

namespace UrlService.Data
{
    public class UrlDbContext : DbContext
    {
        public UrlDbContext(DbContextOptions<UrlDbContext> options) : base(options)
        {
        }
        public DbSet<ShortUrl> ShortUrls { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ShortUrl>(entity =>
            {
                entity.ToTable("ShortUrl");
                entity.HasIndex(u => u.ShortCode).IsUnique();
            });
        }   
    }
}
