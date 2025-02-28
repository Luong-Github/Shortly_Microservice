namespace IdentityService.Models.Requests
{
    public class ReferralRegistrationRequest
    {
        public Guid ReferrerId {  get; set; }
        public Guid ReferredUserId { get; set; }
    }
}
