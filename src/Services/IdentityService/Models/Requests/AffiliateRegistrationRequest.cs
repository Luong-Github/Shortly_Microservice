namespace IdentityService.Models.Requests
{
    public class AffiliateRegistrationRequest
    {
        public Guid BusinessId { get; set; }
        public decimal CommissionRate { get; set; }
    }
}
