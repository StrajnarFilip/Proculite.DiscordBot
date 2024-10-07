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
            return _discordBotContext.AssignRoleMessages.Select(AssignRoleMessageToAssignRoleModel);
        }

        public AssignRoleModel AssignRoleMessageToAssignRoleModel(
            AssignRoleMessage assignRoleMessage
        )
        {
            // TODO: This async operation is execute synchronously.
            string messageContents = _discordService
                .GetGuildMessageContent(
                    assignRoleMessage.MessageGuildId,
                    assignRoleMessage.MessageChannelId,
                    assignRoleMessage.MessageId
                )
                .Result;

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
            string[] parts = messageLink.Split("/").TakeLast(3).ToArray();

            string guildId = parts[0];
            string channelId = parts[1];
            string messageId = parts[2];

            AssignRoleMessage assignRoleMessage = new AssignRoleMessage{
                ChooseOne = chooseOne,
                MessageGuildId = ulong.Parse(guildId),
                MessageChannelId = ulong.Parse(channelId),
                MessageId = ulong.Parse(messageId)
            };
            
            await _discordBotContext.AssignRoleMessages.AddAsync(assignRoleMessage);
            await _discordBotContext.SaveChangesAsync();
        }
    }
}
