namespace Proculite.DiscordBot.Models
{
    public class AssignRoleModel
    {
        public long Id { get; set; }
        public bool ChooseOne { get; set; } = false;
        public ulong MessageGuildId { get; set; }
        public ulong MessageChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string MessageContent { get; set; } = "";
        public string GuildName { get; set; } = "";
    }
}
