using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public class UtilitySlashCommand : ApplicationCommandModule
    {
        private readonly BotContext _botContext;
        public UtilitySlashCommand(BotContext context) 
        {
            _botContext = context;
        }


        [SlashCommand("beep", "ping command!")]
        public async Task PingAsync(InteractionContext ctx)
        {
            await UtilityCommand.PingCommand(ctx);
        }

        [SlashCommand("test", "Test command")]
        public async Task TestAsync(InteractionContext ctx)
        {
            await UtilityCommand.TestCommand(new SlashContext(ctx));
            
        }

 
    }
}
