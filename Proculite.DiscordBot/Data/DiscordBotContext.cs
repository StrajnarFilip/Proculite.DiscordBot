namespace Proculite.DiscordBot.Data
{
    using Microsoft.EntityFrameworkCore;

    public class DiscordBotContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscordBotContext> _logger;

        public DiscordBotContext(IConfiguration configuration, ILogger<DiscordBotContext> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
        }

        private string DatabasePath
        {
            get
            {
                string defaultDatabasePath = "discord-bot.db";
                string? configuredDbPath = _configuration.GetSection("DatabasePath").Get<string>();

                if (configuredDbPath is null)
                {
                    _logger.LogInformation(
                        "Database path is not configured. Using {DefaultPath}.",
                        defaultDatabasePath
                    );
                    return defaultDatabasePath;
                }

                _logger.LogDebug(
                    "Configured database path is: {ConfiguredDbPath}",
                    configuredDbPath
                );
                return configuredDbPath;
            }
        }
    }
}
