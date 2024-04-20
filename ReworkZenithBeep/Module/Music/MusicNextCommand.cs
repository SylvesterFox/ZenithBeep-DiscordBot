

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Music
{
    
    public class MusicNextCommand : BaseCommandModule
    {
        private readonly MusicCommand musicCommand;

        public MusicNextCommand(IServiceProvider serviceProvider) {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            musicCommand = MusicCommand.GetInstance(serviceProvider.GetRequiredService<IAudioService>());
        }

        [Command("join")]
        public async Task JoinCommandAsync(CommandContext ctx)
        {
            await musicCommand.JoinAsync(new NextCommand(ctx));
        }

        [Command("Leave")]
        public async Task LeaveCommandAsync(CommandContext ctx)
        {
            await musicCommand.LeaveAsync(new NextCommand(ctx));
        }
    }
}
