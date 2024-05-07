using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public static class UtilityCommand
    {
        public static async Task TestCommand(CommonContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.RespondTextAsync("Test");
            var message = await ctx.GetOriginalResponseAsync();
            await Task.Delay(1000);
            await message.DeleteAsync();
           
        }

        public static async Task PingCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
            {
                Content = "Beep!"
            });
            Console.WriteLine("test");
            await Task.Delay(6000);
            var message = ctx.GetOriginalResponseAsync();
            await message.Result.DeleteAsync();
        }
    }
}
