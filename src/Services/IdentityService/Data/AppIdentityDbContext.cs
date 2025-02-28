using IdentityService.Entities;
using IdentityService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace IdentityService.Data
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<AppliedDiscount> AppliedDiscounts { get; set; }
        public DbSet<UsageRecord> UsageRecords { get; set; }
        public DbSet<TenantBilling> TenantBilling { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<ReferralReward> ReferralRewards { get; set; }
        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<AffiliateReferral> AffiliateReferrals { get; set; }
        public DbSet<AffiliatePayout> AffiliatePayouts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("User");
            });

            if (_httpContextAccessor.HttpContext?.Items["Tenant"] is Tenant tenant)
            {
                builder.HasDefaultSchema(tenant.Name);
            }
        }
    }
}
