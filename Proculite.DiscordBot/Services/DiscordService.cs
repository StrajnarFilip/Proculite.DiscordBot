using Discord;
using Discord.WebSocket;

namespace Proculite.DiscordBot.Services
{
    public class DiscordService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscordService> _logger;

        public DiscordSocketClient DiscordClient { get; }

        public DiscordService(IConfiguration configuration, ILogger<DiscordService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
            this.DiscordClient = new DiscordSocketClient();
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            string? botToken = _configuration.GetSection("BotToken").Get<string>();

            if (botToken is null)
            {
                _logger.LogCritical("Discord bot token is missing.");
                return;
            }

            await DiscordClient.LoginAsync(TokenType.Bot, botToken);
            await DiscordClient.StartAsync();
        }
    }
}
