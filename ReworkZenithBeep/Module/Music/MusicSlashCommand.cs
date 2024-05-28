
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Music
{
    public class MusicSlashCommand : ApplicationCommandModule
    {
        private readonly MusicCommand musicCommand;
        public MusicSlashCommand(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            musicCommand = MusicCommand.GetInstance(serviceProvider.GetRequiredService<IAudioService>(), serviceProvider);
        }

        [SlashCommand("join", "Connect music bot to voice-channel")]
        public async Task InterctionJoinAsync(InteractionContext ctx)
        {
            await musicCommand.JoinAsync(new SlashContext(ctx));
        }

        [SlashCommand("leave", "Leave from voice channel")]
        public async Task InterctionLeaveAsync(InteractionContext ctx)
        {
            await musicCommand.LeaveAsync(new SlashContext(ctx));
        }

        [SlashCommand("play", "Statring playing")]
        public async Task InterctionPlayAsync(InteractionContext ctx, [Option("url", "Name track")] string query)
        {
            await musicCommand.PlayAsync(new SlashContext(ctx), query);
        }

        [SlashCommand("skip", "Skiping track")]
        public async Task InteractionSkipAsync(InteractionContext ctx, [Option("count", "Counts tracks skip")] long count = 1)
        {
            await musicCommand.SkipAsync(new SlashContext(ctx), count);
        }

        [SlashCommand("queue", "Queue list")]
        public async Task InteractionQueueAsync(InteractionContext ctx)
        {
            await musicCommand.QueueAsync(new SlashContext(ctx));
        }

        [SlashCommand("remove", "Remove track from queue")]
        public async Task InteractionRemoveAsync(InteractionContext ctx, [Option("Nunber", "Removing number track from queue.")] long position)
        {
            await musicCommand.RemoveAsync(new SlashContext(ctx), position);
        }

        [SlashCommand("looptrack", "Looping track")]
        public async Task InteractionLoopAsync(InteractionContext ctx)
        {
            await musicCommand.LoopAsync(new SlashContext(ctx));
        }

        [SlashCommand("pause", "Pause and resume track.")]
        public async Task InteractionPauseAsync(InteractionContext ctx)
        {
            await musicCommand.PauseAsync(new SlashContext(ctx));
        }

        [SlashCommand("clear", "Clear all queue music bot.")]
        public async Task InteractionClearAsync(InteractionContext ctx)
        {
            await musicCommand.ClearAsync(new SlashContext(ctx));
        }
    }

}
