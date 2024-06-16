

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Utils
{
    public class UtilityNextCommand : BaseCommandModule
    {
        private readonly UtilityCommand _Uticommand;
        public UtilityNextCommand(DataBot dataBot)
        {
            _Uticommand = UtilityCommand.GetInstance(dataBot);
        }

        [Command("test")]
        public async Task TestAsyncCommand(CommandContext context)
        {
            await UtilityCommand.TestCommand(new NextCommand(context));
        }

        [Command("prefix")]
        public async Task PrefixAsyncCommand(CommandContext context, string? prefix = null)
        {
            await _Uticommand.PrefixCommand(new NextCommand(context), prefix);
        }

/*        [Command("beep"), Aliases("ping")]
        public async Task BeepAsyncCommand(CommandContext context)
        {
            await UtilityCommand.PingCommand(new NextCommand(context));
        }*/
    }
}
