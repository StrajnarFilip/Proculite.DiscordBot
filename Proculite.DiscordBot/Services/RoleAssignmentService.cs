using Proculite.DiscordBot.Data;
using Proculite.DiscordBot.Models;

namespace Proculite.DiscordBot.Services
{
    public class RoleAssignmentService
    {
        private readonly ILogger<RoleAssignmentService> _logger;
        private readonly DiscordBotContext _discordBotContext;
        private readonly DiscordService _discordService;

        public RoleAssignmentService(
            ILogger<RoleAssignmentService> logger,
            DiscordBotContext discordBotContext,
            DiscordService discordService
        )
        {
            this._logger = logger;
            this._discordBotContext = discordBotContext;
            this._discordService = discordService;
        }

        public IEnumerable<AssignRoleModel> GetAllAssignRoleMessages()
        {
            return _discordBotContext
                .AssignRoleMessages.Select(AssignRoleMessageToAssignRoleModel)
                .Where(message => message is not null);
        }

        public AssignRoleModel? AssignRoleMessageToAssignRoleModel(
            AssignRoleMessage assignRoleMessage
        )
        {
            // TODO: This async operation is execute synchronously.
            string? messageContents = _discordService
                .GetGuildMessageContent(
                    assignRoleMessage.MessageGuildId,
                    assignRoleMessage.MessageChannelId,
                    assignRoleMessage.MessageId
                )
                .Result;

            if (messageContents is null)
            {
                return null;
            }

            string guildName = _discordService.GetGuildName(assignRoleMessage.MessageGuildId);
            return new AssignRoleModel
            {
                Id = assignRoleMessage.Id,
                ChooseOne = assignRoleMessage.ChooseOne,
                MessageGuildId = assignRoleMessage.MessageGuildId,
                MessageChannelId = assignRoleMessage.MessageChannelId,
                MessageId = assignRoleMessage.MessageId,
                MessageContent = messageContents,
                GuildName = guildName
            };
        }

        public async Task AddRoleAssignmentMessageByMessageLink(string messageLink, bool chooseOne)
        {
            string[] allParts = messageLink.Split("/");
            if (allParts.Length < 3)
            {
                _logger.LogWarning(
                    "Message link {MessageLink} contains less than 3 parts.",
                    messageLink
                );
                _logger.LogInformation("Adding new role assignment message is aborted.");
                return;
            }
            string[] parts = allParts.TakeLast(3).ToArray();

            if (parts.Any(part => ulong.TryParse(part, out _)))
            {
                _logger.LogWarning("Message link part is not a number.");
                return;
            }

            if (parts.Any(part => part.Length < 18))
            {
                _logger.LogWarning("Message link part is too short to be valid ID.");
                return;
            }

            ulong guildId = ulong.Parse(parts[0]);
            ulong channelId = ulong.Parse(parts[1]);
            ulong messageId = ulong.Parse(parts[2]);

            if (!_discordService.GuildMessageExists(guildId, channelId, messageId))
            {
                _logger.LogWarning("Message with this ID does not exist: {MessageId}.", messageId);
                return;
            }

            AssignRoleMessage assignRoleMessage = new AssignRoleMessage
            {
                ChooseOne = chooseOne,
                MessageGuildId = guildId,
                MessageChannelId = channelId,
                MessageId = messageId
            };

            await _discordBotContext.AssignRoleMessages.AddAsync(assignRoleMessage);
            await _discordBotContext.SaveChangesAsync();
        }
    }
}
