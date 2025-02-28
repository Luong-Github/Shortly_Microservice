using IdentityService.Models.Requests;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/affiliates")]
    public class AffiliateController : ControllerBase
    {
        private readonly IAffiliateService _affiliateService;

        public AffiliateController(IAffiliateService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAffiliate([FromBody] AffiliateRegistrationRequest request)
        {
            var code = await _affiliateService.RegisterAffiliate(request.BusinessId, request.CommissionRate);
            return Ok(new { ReferralCode = code });
        }

        [HttpPost("referral")]
        public async Task<IActionResult> RegisterReferral([FromBody] RegisterReferralRequest request)
        {
            var success = await _affiliateService.RegisterReferral(request.AffiliateId, request.ReferredUserId, request.OrderAmount);
            return success ? Ok("Referral Registered") : BadRequest("Invalid Affiliate ID");
        }

        [HttpGet("earnings/{affiliateId}")]
        public async Task<IActionResult> GetEarnings(Guid affiliateId)
        {
            var earnings = await _affiliateService.GetAffiliateEarnings(affiliateId);
            return Ok(new { Earnings = earnings });
        }

        [HttpPost("payout")]
        public async Task<IActionResult> ProcessPayout([FromBody] PayoutRequest request)
        {
            var success = await _affiliateService.ProcessPayout(request.AffiliateId, request.PayoutMethod);
            return success ? Ok("Payout Processed") : BadRequest("Invalid Request");
        }
    }
}
