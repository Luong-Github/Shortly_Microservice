using AnalyticsService.Entities;
using AnalyticsService.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Data
{
    public class AppAnalyticsDbContext : DbContext
    {
        public AppAnalyticsDbContext(DbContextOptions<AppAnalyticsDbContext> options) : base(options)
        {
        }

        public DbSet<ClickRecord> ClickRecords { get; set; } = null!;

        public DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClickRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShortCode).IsRequired();
                entity.Property(e => e.IpAddress).IsRequired();
                entity.Property(e => e.UserAgent).IsRequired();
            });
        }
    }
}
