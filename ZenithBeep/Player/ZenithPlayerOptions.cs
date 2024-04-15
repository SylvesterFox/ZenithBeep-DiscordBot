using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players.Vote;


namespace ZenithBeep.Player
{
    public sealed record class ZenithPlayerOptions : VoteLavalinkPlayerOptions
    {
        public ulong? VoiceChannelId { get; set; }
    }
}
