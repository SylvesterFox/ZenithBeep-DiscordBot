
using Discord.Interactions;
using Lavalink4NET;
using ZenithBeep.Player;

namespace ZenithBeep;

public class IntaractionModuleMusic : MusicCmd
{
    public IntaractionModuleMusic(IAudioService audio, MusicZenithHelper zenithHelper) : base(audio, zenithHelper)
    {
    }

    [SlashCommand("join", "Connecting in the voice chat")]
    public async Task Join() {
        await JoinAsync(Context);
    }

    [SlashCommand("leave", "Leave voice channel")]
    public async Task Leave() {
        await LeaveAsync(Context);
    }

    [SlashCommand("play", "Playing song from YouTube")]
    public async Task Play(string query) {
        await PlayAsync(Context, query, false);
    }
}
