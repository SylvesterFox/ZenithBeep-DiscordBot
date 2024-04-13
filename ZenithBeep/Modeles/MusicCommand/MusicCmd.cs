
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using ZenithBeep.Player;

namespace ZenithBeep;

public struct MusicMessage {
    public int Code => code;
    public string Content => content;
    private int code;
    private string content;
    public MusicMessage(int code, string content) {
        this.code = code;
        this.content = content;
    }
}

public abstract class MusicCmd : InteractionModuleBase<SocketInteractionContext>
{
    public readonly IAudioService _audioService;
    protected readonly MusicZenithHelper _musicZenithHelper;



    public MusicCmd(IAudioService audio, MusicZenithHelper zenithHelper)
    {
        _audioService = audio;
        _musicZenithHelper = zenithHelper;
    }



    public async Task JoinAsync(SocketInteractionContext ctx) {
        await ctx.Interaction.DeferAsync(ephemeral: true);


        var player = await _musicZenithHelper.GetPlayerAsync(Context, connectToVoiceChannel: true).ConfigureAwait(false);
        if (player == null)
        {
            return;
        }

        await ctx.Interaction.FollowupAsync($"Connected to  <#{player.VoiceChannelId}>");
    }

    public async Task LeaveAsync(SocketInteractionContext ctx) {
        await ctx.Interaction.DeferAsync(ephemeral: true);
        var player = await _musicZenithHelper.GetPlayerAsync(ctx, connectToVoiceChannel: false).ConfigureAwait(false);
        if (player == null)
        {
            return;
        }

        await player.DisconnectAsync();
        await player.DisposeAsync();
        await ctx.Interaction.FollowupAsync("bye.");
    }

    public async Task PlayAsync(SocketInteractionContext ctx, string query, bool playTop) {
        await ctx.Interaction.DeferAsync();
        var player = await _musicZenithHelper.GetPlayerAsync(ctx);
        if (player is null) return;

        var searchResult = await _audioService.Tracks
                    .LoadTracksAsync(query, TrackSearchMode.YouTube);

        if (searchResult.IsFailed) {
            await ctx.Interaction.FollowupAsync($"Nothing was found for {query}.");
            return;
        }

        if (searchResult.IsPlaylist) {
            await ctx.Interaction.FollowupAsync($"{searchResult.Playlist.Name} Add queue!");

            await player.PlayAsync(searchResult.Track);
            foreach (var track in searchResult.Tracks[1..]) {
                await player.Queue.AddAsync(new TrackQueueItem(track));
            }
            return;
        }

        if(playTop)
        {
            await player.Queue.InsertAsync(0, new TrackQueueItem(searchResult.Track));
        }
        else {
            var playing = await player.PlayAsync(searchResult.Track);
            if (playing > 0) {
                await ctx.Interaction.FollowupAsync($"Add queue `{searchResult.Track.Title}` - {player.Queue.Count}");
            } else {
                await ctx.Interaction.FollowupAsync($"Connected to  <#{player.VoiceChannelId}>");
            }
        }
        
        
    }

    public async Task SkipAsync(SocketInteractionContext ctx, long count)
    {
        await ctx.Interaction.DeferAsync(ephemeral: true);
        var player = await _musicZenithHelper.GetPlayerAsync(ctx).ConfigureAwait(false);
        if (player is null) return;

        if (player.CurrentTrack != null)
        {
            await ctx.Interaction.FollowupAsync($"`{player.CurrentTrack.Title}` skip!");
            await player.SkipAsync((int)count);
            return;
        }

        await ctx.Interaction.FollowupAsync($"Queue empty");
    }

    public async Task SearchAsync(SocketInteractionContext ctx, string query)
    {
        await ctx.Interaction.DeferAsync(ephemeral: false);

        var playlist = await _musicZenithHelper.GetPlayerAsync(ctx); 
        if (playlist is null) return;

        var searchResult = await _audioService.Tracks
            .LoadTracksAsync(query, TrackSearchMode.YouTube);

        if (!searchResult.IsSuccess)
        {
            await ctx.Interaction.FollowupAsync($"Nothing was found for {query}");
            return;
        }

        var tracks = searchResult.Tracks.ToArray();
        playlist.SearchResults = tracks[..5];

        var serchEmbed = PlayerEmbed.SearchEmbed(tracks);

        await ctx.Interaction.FollowupAsync(embed: serchEmbed.Item1, components: serchEmbed.Item2);
    }

}
