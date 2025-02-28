namespace NotificationService.Services
{
    public interface ISlackService
    {
        Task SendSlackMessageAsync(string message);
    }
    public class SlackService : ISlackService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SlackService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendSlackMessageAsync(string message)
        {
            var payload = new
            {
                text = message
            };

            var response = await _httpClient.PostAsJsonAsync(_configuration["Slack:WebhookUrl"], payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
