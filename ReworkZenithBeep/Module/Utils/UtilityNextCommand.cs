

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Utils
{
    public class UtilityNextCommand : BaseCommandModule
    {
        [Command("test")]
        public async Task TestAsyncCommand(CommandContext context)
        {
            await UtilityCommand.TestCommand(new NextCommand(context));
        }

        [Command("beep"), Aliases("ping")]
        public async Task BeepAsyncCommand(CommandContext context)
        {
            await UtilityCommand.PingCommand(new NextCommand(context));
        }
    }
}
