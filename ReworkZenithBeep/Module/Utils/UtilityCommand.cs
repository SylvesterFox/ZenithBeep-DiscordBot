using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public static class UtilityCommand
    {
        public static async Task TestCommand(CommonContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.RespondTextAsync("Test");
        }

        public static async Task PingCommand(CommonContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.RespondTextAsync("boop");
        }
    }
}
