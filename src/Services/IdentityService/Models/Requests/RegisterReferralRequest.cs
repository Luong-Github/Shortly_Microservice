namespace IdentityService.Models.Requests
{
    public class RegisterReferralRequest
    {
        public Guid AffiliateId { get; set; }
        public Guid ReferredUserId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
