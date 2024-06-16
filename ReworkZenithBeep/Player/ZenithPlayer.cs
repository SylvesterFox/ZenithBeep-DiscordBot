using DSharpPlus.Entities;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
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

        protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem track, CancellationToken cancellationToken = default)
        {
            await base
                .NotifyTrackStartedAsync(track, cancellationToken)
                .ConfigureAwait(false);

            var embedPlaying = await EmbedsPlayer.NowPlayingEmbed(track?.Track, "Playing ");
            var message = await channel.SendMessageAsync(embedPlaying);
        }

        public async Task ControlPauseAsync()
        {
            if (IsPaused)
            {
                await ResumeAsync();
                return;
            }

            await PauseAsync();
            await SeekAsync(new TimeSpan(0, 0, -3), SeekOrigin.Current).ConfigureAwait(false);
        }
         
    }
}
