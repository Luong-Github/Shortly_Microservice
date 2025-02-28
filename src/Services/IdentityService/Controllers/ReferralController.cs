using IdentityService.Models.Requests;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/referrals")]
    public class ReferralController : ControllerBase
    {
        private readonly IReferralService _referralService;

        public ReferralController(IReferralService referralService)
        {
            _referralService = referralService;
        }

        [HttpPost("generate/{userId}")]
        public async Task<IActionResult> GenerateReferralCode(Guid userId)
        {
            var code = await _referralService.GenerateReferralCode(userId);
            return Ok(new { ReferralCode = code });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterReferral([FromBody] ReferralRegistrationRequest request)
        {
            var success = await _referralService.RegisterReferral(request.ReferrerId, request.ReferredUserId);
            return success ? Ok("Referral Registered") : BadRequest("Invalid Referral Code");
        }
    }
}
