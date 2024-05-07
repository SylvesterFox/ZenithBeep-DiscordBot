using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            var voiceChannel = context.Guild.GetChannel(player.VoiceChannelId);
            await context.RespondTextAsync($"Connect to `{voiceChannel.Name}`").ConfigureAwait(false);
        }

        public async Task LeaveAsync(CommonContext context)
        {
            var player = await GetPlayerAsync(context, false);
            if (player == null) return;
            var voiceChannel = context.Guild.GetChannel(player.VoiceChannelId);

            await player.DisconnectAsync();
            await player.DisposeAsync();

            await context.RespondTextAsync($"Leave from `{voiceChannel.Name}`. Bye!");
        }

        public async Task PlayAsync(CommonContext ctx, string query)
        {
            await ctx.DeferAsync(false);

            var player = await GetPlayerAsync(ctx);
            if (player == null) return;
            var searchResult = await audioService.Tracks
                .LoadTracksAsync(query, TrackSearchMode.YouTube);

            if (searchResult.IsFailed)
            {
                await ctx.RespondTextAsync($"Nothing was found for {query}.");
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
                await ctx.RespondTextAsync($"Add queue `{searchResult.Track.Title}` - {player.Queue.Count}");
            } else
            {
                await ctx.RespondTextAsync($"Connected to  <#{player.VoiceChannelId}>");
            }
        }

        public async Task SkipAsync(CommonContext ctx, long count)
        {
            var player = await GetPlayerAsync(ctx, false);
            if (player == null) return;

            if (player.CurrentItem != null)
            {
                await ctx.RespondTextAsync($"Skip `{player.CurrentTrack.Title}");
                await player.SkipAsync((int)count);
                return;
            }
            await ctx.RespondTextAsync("Queue empty!");
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
        }

        public async Task QueueAsync(CommonContext ctx)
        {
            await ctx.DeferAsync();
            var player = await GetPlayerAsync(ctx);
            if (player == null) return;

            if (player.Queue.IsEmpty)
            {
                await ctx.RespondTextAsync("The queue is empty");
                return;
            }

            await Pagination.SendMessageAsync(ctx, new PaginationMessage(EmbedsPlayer.QueueEmbed(player),
                    title: "List Queue",
                    embedColor: "#2C2F33",
                    user: ctx.Member,
                    new AppearanceOptions()
                    {
                        Timeout = TimeSpan.FromSeconds(5),
                        Style = DisplayStyle.Full,
                    }));
        }
    }
}
