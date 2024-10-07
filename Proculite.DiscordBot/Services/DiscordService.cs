using System.Text;
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

        public string BotName =>
            new StringBuilder()
                .Append(this.DiscordClient.CurrentUser.Username)
                .Append("#")
                .Append(this.DiscordClient.CurrentUser.Discriminator)
                .ToString();

        public async Task<string?> GetGuildMessageContent(
            ulong guildId,
            ulong channelId,
            ulong messageId
        )
        {
            SocketGuild? guild = DiscordClient.GetGuild(guildId);
            if (guild is null)
            {
                _logger.LogWarning(
                    "Attempting to get guild that isn't accessible or doesn't exist: {GuildId}",
                    guildId
                );
                return null;
            }

            SocketTextChannel? textChannel = guild.GetTextChannel(channelId);
            if (textChannel is null)
            {
                _logger.LogWarning(
                    "Attempting to get channel that isn't accessible or doesn't exist: {ChannelId}",
                    channelId
                );
                return null;
            }

            IMessage? message = await textChannel.GetMessageAsync(messageId);

            if (message is null)
            {
                _logger.LogWarning("Message with ID {MessageId} has not been found.", messageId);
                return null;
            }
            return message.Content;
        }

        public string GetGuildName(ulong guildId)
        {
            return DiscordClient.GetGuild(guildId).Name;
        }

        public bool GuildMessageExists(ulong guildId, ulong channelId, ulong messageId)
        {
            SocketGuild? guild = DiscordClient.GetGuild(guildId);
            if (guild is null)
                return false;

            SocketTextChannel? textChannel = guild.GetTextChannel(channelId);
            if (textChannel is null)
                return false;

            return textChannel.GetMessageAsync(messageId).Result != null;
        }
    }
}
