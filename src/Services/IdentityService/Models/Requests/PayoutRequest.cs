namespace IdentityService.Models.Requests
{
    public class PayoutRequest
    {
        public Guid AffiliateId { get; set; }
        public string PayoutMethod { get; set; }
    }
}
