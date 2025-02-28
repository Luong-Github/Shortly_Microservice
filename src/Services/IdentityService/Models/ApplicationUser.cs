using IdentityService.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public Guid TenantId { get; set; } // Each user belongs to a tenant
        public Tenant Tenant { get; set; }
    }
}
