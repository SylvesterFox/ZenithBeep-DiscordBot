
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

    public async ValueTask<ZenithPlayer> GetPlayerAsync(SocketInteractionContext ctx, bool connectToVoiceChannel = true)
        {
            var channelBehavior = connectToVoiceChannel ? PlayerChannelBehavior.Move : PlayerChannelBehavior.None;
            var member = Context.Client.GetMember(Context.User.Id, Context.Guild.Id);
            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior, VoiceStateBehavior: MemberVoiceStateBehavior.Ignore);

            var result = await _audioService.Players
                .RetrieveAsync<ZenithPlayer, ZenithPlayerOptions>(
                    guildId: ctx.Guild.Id,
                    memberVoiceChannel: member.VoiceChannel.Id,
                    playerFactory: ZenithPlayer.CreatePlayerAsync,
                    options: Options.Create(new ZenithPlayerOptions()
                    {
                        VoiceChannel = member.VoiceChannel,
                    }),
                    retrieveOptions: retrieveOptions
                );

            if (!result.IsSuccess)
            {
                
            var errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.Success => "Success",
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel",
                    PlayerRetrieveStatus.VoiceChannelMismatch => "You are not in the same channel as the Music Bot!",
                    PlayerRetrieveStatus.UserInSameVoiceChannel => "Same voice channel?",
                    PlayerRetrieveStatus.BotNotConnected => "The bot is currenly not connected.",
                    PlayerRetrieveStatus.PreconditionFailed => "A unknown error happened: Precondition Failed.",
                    _ => "A unknown error happened"
                };
                await ctx.Interaction.FollowupAsync(errorMessage);
                return null;
            }


            return result.Player;
        }

    public async Task JoinAsync(SocketInteractionContext ctx) {
        await ctx.Interaction.DeferAsync(ephemeral: true);
        // var voiceState = guildUser.VoiceState;
        // if (voiceState is null || voiceState.Value.VoiceChannel is null || guildUser is null)
        // {
        //     await FollowupAsync("Not a vaild voice channel", ephemeral: true);
        //     return;
        // }

        // if (voiceState.Value.VoiceChannel.Guild.Id != Context.Guild.Id)
        // {
        //     await FollowupAsync("Not in voice channel of this guild", ephemeral: true);
        //     return;
        // }

        var player = await GetPlayerAsync(Context, connectToVoiceChannel: true).ConfigureAwait(false);
        if (player == null)
        {
            return;
        }

        await ctx.Interaction.FollowupAsync($"Connected to  <#{player.VoiceChannelId}>");
    }

    public async Task LeaveAsync(SocketInteractionContext ctx) {
        await ctx.Interaction.DeferAsync(ephemeral: true);
        var player = await GetPlayerAsync(ctx, connectToVoiceChannel: false).ConfigureAwait(false);
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
        var player = await GetPlayerAsync(ctx);
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

    }

}
