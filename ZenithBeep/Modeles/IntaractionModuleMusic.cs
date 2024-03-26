
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Rest.Entities.Tracks;
using ZenithBeep.Modeles;
using ZenithBeepData;

namespace ZenithBeep;

public class IntaractionModuleMusic : ZenithBase
{
    private readonly IAudioService _audioService;

    public IntaractionModuleMusic(DataAccessLayer dataAccessLayer, IAudioService audioService) : base(dataAccessLayer)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        _audioService = audioService;
    }

    [SlashCommand("disconnect", "Disconnects from the current voice channel connected", runMode: RunMode.Async)]
<<<<<<< HEAD
    public async Task Disconnect()
    {
        var member = Context.Guild.GetUser(Context.User.Id);
        var voiceState = member.VoiceState;
        await DeferAsync(ephemeral: true).ConfigureAwait(false);
=======
    public async Task Disconnect() {
        var player = await GetPlayerAsync().ConfigureAwait(false);
>>>>>>> parent of b9a63d5 (Dev-commit #2)

        if (player is null) {
            return;
        }

        await player.DisconnectAsync().ConfigureAwait(false);
    }

    [SlashCommand("play", "Starting play music", runMode: RunMode.Async)]
    public async Task Play(String query) {
        await DeferAsync(ephemeral: true).ConfigureAwait(false);

        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

        if (player is null) {
            return;
        }

        var track = await _audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        if (track is null) {
            await FollowupAsync("No results").ConfigureAwait(false);
            return;
        }

        var position = await player.PlayAsync(track).ConfigureAwait(false);

        if (position is 0){
            await FollowupAsync($"Playing: {track.Uri}").ConfigureAwait(false);
        } else {
            await FollowupAsync($"Added to queue: {track.Uri}").ConfigureAwait(false);
        }
    }

<<<<<<< HEAD
    private async Task _play_Single(ZenithPlayer player, LavalinkTrack? track) {
        if (track is null)
        {
            await FollowupAsync($"Failed to parse track", ephemeral: true);
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
=======
    private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true) {
>>>>>>> parent of b9a63d5 (Dev-commit #2)
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
    }

}
