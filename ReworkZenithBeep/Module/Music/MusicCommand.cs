using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReworkZenithBeep.MessageEmbeds;
using ReworkZenithBeep.Player;
using ReworkZenithBeep.Services;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Music
{
    public partial class MusicCommand
    {
        public PaginationService Pagination;

        private readonly IAudioService audioService;
        private static MusicCommand instance;

        private MusicCommand(IAudioService audioService, IServiceProvider service)
        {
            ArgumentNullException.ThrowIfNull(audioService);
            this.audioService = audioService;
            this.Pagination = service.GetRequiredService<PaginationService>();
        }

        

        public static MusicCommand GetInstance(IAudioService audioService, IServiceProvider service)
        {
            if (instance == null)
            {
                instance = new MusicCommand(audioService, service);
            }
            return instance;
        }

        private async ValueTask<ZenithPlayer?> GetPlayerAsync(CommonContext ctx, bool connectToVoiceChannel = true)
        {
            var options = new ZenithPlayerOptions() { Context = ctx, SelfDeaf = true, HistoryCapacity = 10 };
            var retrieveOptions = new PlayerRetrieveOptions(
                    ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Move : PlayerChannelBehavior.None,
                    VoiceStateBehavior: MemberVoiceStateBehavior.Ignore);

            PlayerResult<ZenithPlayer> result;
            try
            {
                result = await audioService.Players
                    .RetrieveAsync<ZenithPlayer, ZenithPlayerOptions>(ctx.Guild.Id, ctx.Member.VoiceState?.Channel?.Id,
                    ZenithPlayer.CreatePlayerAsync,
                    Options.Create(options),
                    retrieveOptions
                    ).ConfigureAwait(false);
            } catch (TimeoutException)
            {
                await ctx.RespondTextAsync("Timeout player error");
                return null;
            }

            if (!result.IsSuccess)
            {
                var message = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel",
                    _ => "A unknown error happened"
                };
            }

            return result.Player;
        }

        public async Task JoinAsync(CommonContext context)
        {  
            var player = await GetPlayerAsync(context); if (player == null) return;
            var embed = EmbedTempalte.UniEmbed($"Connected to  <#{player.VoiceChannelId}>");
            await context.RespondEmbedAsync(embed);
        }

        public async Task LeaveAsync(CommonContext context)
        {
            var player = await GetPlayerAsync(context, false);
            if (player == null) return;
            var voiceChannel = context.Guild.GetChannel(player.VoiceChannelId);

            await player.DisconnectAsync();
            await player.DisposeAsync();
            var embed = EmbedTempalte.UniEmbed($"Leave from `{voiceChannel.Name}`. Bye!", "#7cf66e");
            await context.RespondEmbedAsync(embed).ConfigureAwait(false);
        }

        public async Task PlayAsync(CommonContext ctx, string query)
        {
            await ctx.DeferAsync(true);

            var player = await GetPlayerAsync(ctx);
            if (player == null) return;
            var searchResult = await audioService.Tracks
                .LoadTracksAsync(query, TrackSearchMode.YouTube);

            if (searchResult.IsFailed)
            {;
                var embed = EmbedTempalte.UniEmbed($"Nothing was found for {query}.");
                await ctx.RespondEmbedAsync(embed);
                return;
            }

            if (searchResult.IsPlaylist)
            {
                await player.PlayAsync(searchResult.Track);
                foreach (var track in searchResult.Tracks[1..]) { 
                    await player.Queue.AddAsync(new TrackQueueItem(track));
                }
                return;
            }

            var playing = await player.PlayAsync(searchResult.Track);
            if (playing > 0)
            {
                var embed = EmbedTempalte.UniEmbed($"Add queue `{searchResult.Track.Title}` - {player.Queue.Count}");
                await ctx.RespondEmbedAsync(embed);
            } else
            {
                var embed = EmbedTempalte.UniEmbed($"Connected to  <#{player.VoiceChannelId}>");
                await ctx.RespondEmbedAsync(embed);

            }
        }

        public async Task SkipAsync(CommonContext ctx, long count)
        {
            var player = await GetPlayerAsync(ctx, false);
            if (player == null) return;

            if (player.CurrentItem != null)
            {
                var embed_skip = EmbedTempalte.UniEmbed($"Skip `{player.CurrentTrack.Title}");
                await ctx.RespondEmbedAsync(embed_skip);
                await player.SkipAsync((int)count);
                return;
            }
            var embed = EmbedTempalte.UniEmbed("Queue empty!");
            await ctx.RespondEmbedAsync(embed);
        }

        public async Task PauseAsync(CommonContext ctx)
        {
            var player = await GetPlayerAsync(ctx);
            if (player == null) return;

            if (player.CurrentItem != null)
            {
                string answer = player.IsPaused ? "Let's continue" : "Suspended";
                await player.ControlPauseAsync();
                await ctx.RespondTextAsync(answer);
            }
            else await ctx.RespondEmbedAsync(EmbedsPlayer.EmptyQueueEmbed());
        }

        public async Task QueueAsync(CommonContext ctx)
        {
            await ctx.DeferAsync();
            var player = await GetPlayerAsync(ctx);
            if (player == null) return;

            if (player.Queue.IsEmpty)
            {
                var embed = EmbedTempalte.UniEmbed("The queue is empty");
                await ctx.RespondEmbedAsync(embed);
                return;
            }

            await Pagination.SendMessageAsync(ctx, new PaginationMessage(EmbedsPlayer.QueueEmbed(player),
                    title: "List Queue",
                    embedColor: "#2C2F33",
                    user: ctx.Member,
                    new AppearanceOptions()
                    {
                        Timeout = TimeSpan.FromSeconds(500),
                        Style = DisplayStyle.Full,
                    }));
        }

        public async Task RemoveAsync(CommonContext ctx, long position)
        {
            await ctx.DeferAsync(ephemeral: true);
            var player = await GetPlayerAsync(ctx);
            if (player == null) return;

            if(player.Queue.Count > position - 1)
            {
                var embed = EmbedTempalte.UniEmbed($"`{position}.` {player.Queue[(int)position - 1].Track.Title} remove from queue");
                await ctx.RespondEmbedAsync(embed);
                await player.Queue.RemoveAtAsync((int)position - 1).ConfigureAwait(false);
            } else
            {
                var embed = EmbedTempalte.UniEmbed($"Unable to delete track `{position}`. Wrong number.");
                await ctx.RespondEmbedAsync(embed);
            }
        }

        public async Task LoopAsync(CommonContext ctx)
        {
            await ctx.DeferAsync(ephemeral: true);
            var player = await GetPlayerAsync(ctx);
            if (player == null) return;

            if (player.CurrentTrack != null)
            {
                player.RepeatMode = player.RepeatMode == TrackRepeatMode.Track ? TrackRepeatMode.None : TrackRepeatMode.Track;
                await ctx.RespondTextAsync($"Track {(player.RepeatMode == TrackRepeatMode.Track ? "looped!" : "not looped!")}");
            }
            else
                await ctx.RespondEmbedAsync(EmbedsPlayer.EmptyQueueEmbed());
        }

        public async Task ClearAsync(CommonContext ctx)
        {
            var player = await GetPlayerAsync(ctx, false);
            if (player == null) return;

            await player.StopAsync().ConfigureAwait(false);
            await player.Queue.ClearAsync().ConfigureAwait(false);
            var embed = EmbedTempalte.UniEmbed("Clear queue!");
            await ctx.RespondEmbedAsync(embed);
        }

   

    }
}
