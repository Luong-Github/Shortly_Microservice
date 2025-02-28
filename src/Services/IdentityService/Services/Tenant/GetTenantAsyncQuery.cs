using MediatR;
using IdentityService.Entities;
using Tenants = IdentityService.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using IdentityService.Data;
namespace IdentityService.Services.Tenant
{
    public class GetTenantAsyncQuery : IRequest<Tenants>
    {
        public string Domain { get; set; }
    }

    public class GetTenantAsyncQueryHandler : IRequestHandler<GetTenantAsyncQuery, Tenants>
    {
        private readonly AppIdentityDbContext _context;

        public GetTenantAsyncQueryHandler(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task<Tenants> Handle(GetTenantAsyncQuery request, CancellationToken cancellationToken)
        {
            return await _context.Tenants.FirstOrDefaultAsync(t => t.Domain == request.Domain);
        }
    }
}
