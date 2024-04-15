﻿
using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ZenithBeep.Player
{
    public sealed class ZenithPlayer : VoteLavalinkPlayer
    {
        public LavalinkTrack?[] SearchResults { set => searchList = value; }

        private readonly ulong _idChannel;
        private readonly DiscordSocketClient discordClient;
        private LavalinkTrack?[] searchList = new LavalinkTrack?[5];
        public ZenithPlayer(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties) : base(properties)
        {
            _idChannel = properties.VoiceChannelId;
            discordClient = properties.ServiceProvider!.GetRequiredService<DiscordSocketClient>();
        }

        public static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> playerProperties, CancellationToken cancellation = default) {
            cancellation.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(playerProperties);

            return ValueTask.FromResult(new ZenithPlayer(playerProperties));
        }

        public LavalinkTrack?[] GetSearchResults() => (LavalinkTrack?[]) searchList.Clone();

        protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem tqi, CancellationToken cancellationToken = default)
        {
            await base
                .NotifyTrackStartedAsync(tqi, cancellationToken)
                .ConfigureAwait(false);

           
            var embed_music = await PlayerExtensions.GetEmbedAsync(tqi.Track);
            var channelVoice = await discordClient.GetChannelAsync(_idChannel) as IMessageChannel;

            if (channelVoice == null)
            {
                return;
            }

            await channelVoice.SendMessageAsync("", embed: embed_music);

            // send a message to the text channel
            Log.Debug($"Starting playing song this channel: [{channelVoice.Id}]{channelVoice.Name}:{tqi.Track.Title}");
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

        protected override async ValueTask NotifyTrackExceptionAsync(ITrackQueueItem track, TrackException exception, CancellationToken cancellationToken = default) {
            await base
                .NotifyTrackExceptionAsync(track, exception, cancellationToken)
                .ConfigureAwait(false);

            var channelVoice = await discordClient.GetChannelAsync(_idChannel) as IMessageChannel;

            if (channelVoice == null)
            {
                return;
            }

            await channelVoice.SendMessageAsync($"{exception}: {exception.Message}");
            
        }
    }
}
