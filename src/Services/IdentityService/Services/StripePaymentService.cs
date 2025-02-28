using Stripe;

namespace IdentityService.Services
{
    public class StripePaymentService
    {
        public static async Task<bool> ProcessPayment(string token, decimal amount)
        {
            StripeConfiguration.ApiKey = "sk_test_your_secret_key";

            var options = new ChargeCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = "usd",
                Source = token,
                Description = "Subscription Payment"
            };

            var service = new ChargeService();
            var charge = await service.CreateAsync(options);
            return charge.Status == "succeeded";
        }
    }
}
