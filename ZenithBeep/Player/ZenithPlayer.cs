
using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ZenithBeep.Player
{
    public sealed class ZenithPlayer : VoteLavalinkPlayer
    {
        private readonly SocketVoiceChannel _ChannelVoice;
        private readonly DiscordSocketClient discordClient;
        public ZenithPlayer(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties) : base(properties)
        {
            _ChannelVoice = properties.Options.Value.VoiceChannel;
            discordClient = properties.ServiceProvider!.GetRequiredService<DiscordSocketClient>();
        }

        public static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> playerProperties, CancellationToken cancellation = default) {
            cancellation.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(playerProperties);

            return ValueTask.FromResult(new ZenithPlayer(playerProperties));
        }

        protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem tqi, CancellationToken cancellationToken = default)
        {
            await base
                .NotifyTrackStartedAsync(tqi, cancellationToken)
                .ConfigureAwait(false);

            if (State == PlayerState.Playing)
            {
                Console.WriteLine("Playling");
            }

           
            var embed_music = await LavaExtension.GetEmbedAsync(tqi.Track, "Playing");

            await _ChannelVoice.SendMessageAsync("", embed: embed_music.Build());

            // send a message to the text channel
            Log.Debug($"Starting playing song this channel: [{_ChannelVoice.Id}]{_ChannelVoice.Name}:{tqi.Track.Title}");
        }

        protected override async ValueTask NotifyTrackExceptionAsync(ITrackQueueItem track, TrackException exception, CancellationToken cancellationToken = default) {
            await base
                .NotifyTrackExceptionAsync(track, exception, cancellationToken)
                .ConfigureAwait(false);

            await _ChannelVoice.SendMessageAsync($"{exception}: {exception.Message}");
            
        }
    }
}
