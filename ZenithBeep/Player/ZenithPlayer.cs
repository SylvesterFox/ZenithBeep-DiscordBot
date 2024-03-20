using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ZenithBeepData;

namespace ZenithBeep.Player
{
    public sealed class ZenithPlayer : VoteLavalinkPlayer
    {
        private readonly ulong _idChannel;
        private readonly DiscordSocketClient discordClient;
        private readonly DataAccessLayer DataAccessLayer;
        private readonly MusicZenithHelper musicZenithHelper;

        public ZenithPlayer(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties) : base(properties)
        {
            _idChannel = properties.VoiceChannelId;
            discordClient = properties.ServiceProvider!.GetRequiredService<DiscordSocketClient>();
            musicZenithHelper = properties.ServiceProvider!.GetRequiredService<MusicZenithHelper>();
            DataAccessLayer = properties.ServiceProvider!.GetRequiredService<DataAccessLayer>();
        }

        protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem tqi, CancellationToken cancellationToken = default)
        {
            await base
                .NotifyTrackStartedAsync(tqi, cancellationToken)
                .ConfigureAwait(false);

            musicZenithHelper.AnnounceJoin(GuildId, VoiceChannelId);

            var track = tqi.Track;
            if (track == null)
            {
                return;
            }

            var discordGuild = await GetGuildAsync();
            var dbGuild = await DataAccessLayer.GetOrCreateGuild(_guild);
            var guildState = musicZenithHelper.GetOrCreateGuildState(dbGuild.Id);

            if (guildState == null)
            {
                guildState = new GuildState(dbGuild.Id);
                musicZenithHelper.GuildStates.Add(dbGuild.Id, guildState);
            }

            guildState.TrackChapters = null;

            var outputChannel = await musicZenithHelper.GetMusicTextChannelFor(discordGuild);
            if (outputChannel == null)
            {
                Log.Error("Failed to get music channel");
            } else
            {
                await musicZenithHelper.DeletePastStatusMessage(dbGuild, outputChannel);
            }

            
        }

        #region Guild and Track Functions
        private SocketGuild _guild;
        private async Task<SocketGuild> GetGuildAsync()
        {
            return _guild ??= discordClient.GetGuild(GuildId);
        }
        #endregion
    }
}
