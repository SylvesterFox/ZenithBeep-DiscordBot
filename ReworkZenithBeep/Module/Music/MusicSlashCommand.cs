
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
            musicCommand = MusicCommand.GetInstance(serviceProvider.GetRequiredService<IAudioService>());
        }

        [SlashCommand("join", "Connect music bot to voice-channel")]
        public async Task InterctionJoinAsync(InteractionContext ctx)
        {
            await musicCommand.JoinAsync(new SlashContext(ctx));
        }

        [SlashCommand("Leave", "Leave from voice channel")]
        public async Task InterctionLeaveAsync(InteractionContext ctx)
        {
            await musicCommand.LeaveAsync(new SlashContext(ctx));
        }
    }
}
