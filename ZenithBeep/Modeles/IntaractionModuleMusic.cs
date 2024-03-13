
using Discord;
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
    public async Task Disconnect() {
        var player = await GetPlayerAsync().ConfigureAwait(false);

        if (player is null) {
            return;
        }

        await player.DisconnectAsync().ConfigureAwait(false);
    }

    [SlashCommand("play", "Starting play music", runMode: RunMode.Async)]
    public async Task Play(String query) {
        await DeferAsync().ConfigureAwait(false);

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

    private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true) {
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
