using DSharpPlus.Entities;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Player
{
    public sealed record class ZenithPlayerOptions : VoteLavalinkPlayerOptions
    {
        public CommonContext Context { get; init; }
    }


    public class ZenithPlayer : VoteLavalinkPlayer
    {
        public DiscordChannel Channel => channel;


        private readonly DiscordChannel channel;
        private DiscordMessage? message;

        private ZenithPlayer(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellation = default) : base(properties)
        {
            this.channel = properties.Options.Value.Context.Channel;
        }

        public static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(nameof(properties));

            return ValueTask.FromResult(new ZenithPlayer(properties));
        }
         
    }
}
