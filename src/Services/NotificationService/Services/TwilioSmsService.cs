using NotificationService.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public TwilioSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
            TwilioClient.Init(_configuration["Twilio:AccountSid"], _configuration["Twilio:AuthToken"]);
        }

        public async Task SendSmsAsync(string to, string message)
        {
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_configuration["Twilio:From"]),
                to: new PhoneNumber(to)
            );

            Console.WriteLine($"SMS Sent: {messageResource.Sid}");
        }
    }
}
