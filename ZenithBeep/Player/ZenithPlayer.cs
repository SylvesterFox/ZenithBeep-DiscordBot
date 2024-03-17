using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ZenithBeep.Player
{
    public sealed class ZenithPlayer : VoteLavalinkPlayer
    {
        private readonly ulong _idChannel;
        private readonly DiscordSocketClient discordClient;
        public ZenithPlayer(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties) : base(properties)
        {
            _idChannel = properties.VoiceChannelId;
            discordClient = properties.ServiceProvider!.GetRequiredService<DiscordSocketClient>();
        }

        protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem track, CancellationToken cancellationToken = default)
        {
            await base
                .NotifyTrackStartedAsync(track, cancellationToken)
                .ConfigureAwait(false);

            if (State == PlayerState.Playing)
            {
                Console.WriteLine("Playling");
            }

            var channel = await discordClient.GetChannelAsync(_idChannel);
            // send a message to the text channel
            Log.Debug($"Starting playing song this channel: [{_idChannel}]{channel.Name}:{track.Track.Title}");
        }
    }
}
