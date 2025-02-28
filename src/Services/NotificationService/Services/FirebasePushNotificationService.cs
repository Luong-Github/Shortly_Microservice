using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace NotificationService.Services
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(string token, string title, string body);
    }
    public class FirebasePushNotificationService : IPushNotificationService
    {
        public FirebasePushNotificationService(IConfiguration configuration)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(configuration["Firebase:JsonPath"])
                });
            }
        }

        public async Task SendPushNotificationAsync(string token, string title, string body)
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine($"Push Notification Sent: {response}");
        }
    }
}
