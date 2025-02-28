namespace NotificationService.Services
{
    public interface ITelegramService
    {
        Task SendTelegramMessageAsync(string message);
    }

    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TelegramService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendTelegramMessageAsync(string message)
        {
            var telegramUrl = $"https://api.telegram.org/bot{_configuration["Telegram:BotToken"]}/sendMessage";
            var parameters = new Dictionary<string, string>
        {
            { "chat_id", _configuration["Telegram:ChatId"] },
            { "text", message }
        };

            var response = await _httpClient.PostAsJsonAsync(telegramUrl, parameters);
            response.EnsureSuccessStatusCode();
        }
    }
}
