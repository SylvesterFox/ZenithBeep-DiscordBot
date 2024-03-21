using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Runtime.Serialization;
using ZenithBeepData;
using ZenithBeepData.Models;

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

            ModelGuildQueueItem? dbTrack = null;

            var currentTrackIdx = dbGuild.CurrentTrack;

            var requstedBy = "<#ERROR>";
            var db = DataAccessLayer._contextFactory.CreateDbContext();
            var currentTrackQuery = db.GuildQueueItems.Include(p => p.RequestedBy).Where(x => x.GuildId == dbGuild.Id && x.Position == currentTrackIdx);
            if (currentTrackQuery.Any())
            {
                dbTrack = await currentTrackQuery.FirstAsync(cancellationToken);
                requstedBy = (dbTrack?.RequestedBy == null) ? "<#NULL>" : dbTrack?.RequestedBy.DisplayName;
            }

            var musicEmbed = await LavaExtension.MusicEmbed(track, "Playing", dbTrack: dbTrack, requestedBy: requstedBy);

            if (guildState.ShuffleEnbled)
                musicEmbed.AddField("Shuffle", "Enabled", true);

            if (guildState.RepeatEnabled)
                musicEmbed.AddField("Repeat", $"Repeated `{guildState.TimesRepeated}` time", true);

            var embedIndex = musicEmbed.Fields.Count;
            var message = await outputChannel.SendMessageAsync(embed: musicEmbed.Build());

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
