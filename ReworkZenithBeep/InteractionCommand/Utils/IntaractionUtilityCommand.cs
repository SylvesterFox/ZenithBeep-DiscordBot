using DSharpPlus.SlashCommands;


namespace ReworkZenithBeep.InteractionCommand.Utils
{
    public class IntaractionUtilityCommand : ApplicationCommandModule
    {
        [SlashCommand("beep", "ping command!")]
        public async Task PingAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("boop!"));
        }
    }
}
