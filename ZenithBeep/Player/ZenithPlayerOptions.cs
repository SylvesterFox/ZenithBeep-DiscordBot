using Discord;
using Lavalink4NET.Players.Vote;


namespace ZenithBeep.Player
{
    public sealed record class ZenithPlayerOptions : VoteLavalinkPlayerOptions
    {
        public ITextChannel TextChannel { get; }
        public ulong? VoiceChannelId { get; set; }
    }
}
