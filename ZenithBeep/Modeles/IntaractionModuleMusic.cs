
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using ZenithBeep.Modeles;
using ZenithBeep.Player;

namespace ZenithBeep;

public class IntaractionModuleMusic : BaseAudioCommandModule
{

    public IntaractionModuleMusic(IAudioService audioService, MusicZenithHelper musicZenithHelper) : base(audioService, musicZenithHelper)
    {

    }

    static bool IsUrl(string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    [SlashCommand("disconnect", "Disconnects from the current voice channel connected", runMode: RunMode.Async)]
    public async Task Disconnect()
    {
        var member = Context.Guild.GetUser(Context.User.Id);
        var voiceState = member.VoiceState;
        await DeferAsync().ConfigureAwait(false);

        if (member?.VoiceState == null || member?.VoiceState.Value.VoiceChannel.Id == null)
        {
            await FollowupAsync("You are not in a voice channel.");
            return;
        }

        var voiceChannel = voiceState.Value.VoiceChannel;
        (var playerState, var playerIsConnected) = await GetPlayerAsync(Context.Guild.Id, voiceChannel.Id, connectToVoiceChannel: false);

        if (playerIsConnected && playerState.Player != null)
        {
            await playerState.Player.DisconnectAsync().ConfigureAwait(false);

            await FollowupAsync($"Left <#{member?.VoiceState.Value.VoiceChannel.Id}>");
        } else
        {
            await FollowupAsync("Music bot is not connected.");
        }
    }

    [SlashCommand("play", "Starting play music", runMode: RunMode.Async)]
    public async Task Play(String query) {
        await DeferAsync().ConfigureAwait(false);

        var member = Context.Guild.GetUser(Context.User.Id);
        var voiceState = member.VoiceState;

        if (voiceState is null || voiceState.Value.VoiceChannel is null || member is null)
        {
            await FollowupAsync("Not a vaild voice channel");
            return;
        }

        if (voiceState.Value.VoiceChannel.Guild.Id != Context.Guild.Id)
        {
            await FollowupAsync("Not in voice channel of this guild");
            return;
        }


        var voiceChannel = voiceState.Value.VoiceChannel;
        (var playerState, var playerIsConnected) = await GetPlayerAsync(Context.Guild.Id, voiceChannel.Id, connectToVoiceChannel: true).ConfigureAwait(false);
        if (playerIsConnected == false || playerState.Player == null)
        {
            await FollowupAsync(GetPlayerErrorMessage(playerState.Status));
            return;
        }

        await FollowupAsync($"Connected to <#{member?.VoiceState.Value.VoiceChannel.Id}>");
        bool isYouTubeUrl = false;
        TrackLoadResult track;

        if (IsUrl(query))
        {
            isYouTubeUrl = true;

            track = await _audioService.Tracks
                .LoadTracksAsync(query, TrackSearchMode.None)
                .ConfigureAwait(false);
        } else
        {
            track = await _audioService.Tracks
                .LoadTracksAsync(query, TrackSearchMode.YouTube)
                .ConfigureAwait(false);
        }

        await _play_Single(playerState.Player, track.Track);
    }

    private async Task _play_Single(ZenithPlayer player, LavalinkTrack? track) {
        if (track is null)
        {
            await FollowupAsync($"Failed to parse track");
            return;
        }

        await FollowupAsync($"Playing `{track.Title}`");
        await player.PlayAsync(track).ConfigureAwait(false);
    }

/*    [SlashCommand("queue", "Get queue list", runMode: RunMode.Async)]
    public async Task ListQueue()
    {
        await DeferAsync().ConfigureAwait(false);

        var player = await GetPlayerAsync(connectToVoiceChannel: false);

        if (player is null)
            return;

        if (player.Queue.IsEmpty)
        {
            await SendEmbedAsync(
                    title: "Queue",
                    description: "_... Empty ..._",
                    color: Color.DarkPurple
                );
            return;
        }

        // List<EmbedBuilder> builders = 
    }*/



    /*private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true) {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None
        );

        var result = await _audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Vote, retrieveOptions: retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess) {
            var errorMessage = result.Status switch {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected",
                _ => "Unknown error"
            };

            await SendEmbedAsync("Music error", errorMessage, color: Color.Red, ephemeral: true);
            return null;
        }

        return result.Player;
    }*/

}
