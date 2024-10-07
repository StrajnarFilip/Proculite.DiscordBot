namespace Proculite.DiscordBot.Models
{
    public class AssignRoleMessage
    {
        public long Id { get; set; }
        /// <summary>
        /// If true, only one role can be assigned from
        /// reaction to this message.
        /// </summary>
        public bool ChooseOne { get; set; } = false;
        public ulong MessageGuildId { get; set; }
        public ulong MessageChannelId { get; set; }
        public ulong MessageId { get; set; }
        public List<ReactionRole> ReactionRoles { get; set; } = new();
    }
}