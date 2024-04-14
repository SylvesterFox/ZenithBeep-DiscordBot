
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

    [SlashCommand("skip", "Skip track")]
    public async Task Skip(long count = 1) { await SkipAsync(Context, count); }

    [SlashCommand("search", "Serach track")]
    public async Task Search(string query)
    {
        await DeferAsync(ephemeral: false);
        await SearchAsync(Context, query);
    }


}
