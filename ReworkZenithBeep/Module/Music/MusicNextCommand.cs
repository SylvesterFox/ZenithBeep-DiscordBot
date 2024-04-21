

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

        [Command("leave")]
        public async Task LeaveCommandAsync(CommandContext ctx)
        {
            await musicCommand.LeaveAsync(new NextCommand(ctx));
        }

        [Command("play")]
        public async Task PlayCommandAsync(CommandContext ctx, string query)
        {
            await musicCommand.PlayAsync(new NextCommand(ctx), query);
        }

        [Command("skip")]
        public async Task SkipCommandAsync(CommandContext ctx, long count = 1)
        {
            await musicCommand.SkipAsync(new NextCommand(ctx), count);
        }


    }
}
