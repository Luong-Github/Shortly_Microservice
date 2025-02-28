using IdentityService.Data;
using IdentityService.Entities;
using IdentityService.Services.Discount;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/discounts")]
    public class DiscountController : ControllerBase
    {
        private readonly AppIdentityDbContext _context;
        private readonly IMediator _mediator;
        public DiscountController(AppIdentityDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountCode discount)
        {
            _context.Set<DiscountCode>().Add(discount);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllDiscounts()
        {
            var discounts = await _context.Set<DiscountCode>().ToListAsync();
            return Ok(discounts);
        }

        [HttpPost("apply/{tenantId}")]
        public async Task<IActionResult> ApplyDiscount(Guid tenantId, [FromBody] string code)
        {
            var result = await _mediator.Send(new ApplyDiscountCommand() { Code=code, TenantId = tenantId});
            return result ? Ok("Discount Applied") : BadRequest("Invalid or Expired Code");
        }
    }
}
