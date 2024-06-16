using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public partial class UtilityCommand
    {
        
        private readonly DataBot _dbContext;
        private static UtilityCommand instance;

        public UtilityCommand(DataBot dbContext)
        {
            this._dbContext = dbContext;
        }

        public static UtilityCommand GetInstance(DataBot dbContext)
        {
            if (instance == null)
            {
                instance = new UtilityCommand(dbContext);
            }
            return instance;
        }

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
            await Task.Delay(6000);
            var message = ctx.GetOriginalResponseAsync();
            await message.Result.DeleteAsync();
        }

        public async Task PrefixCommand(CommonContext ctx, string? prefix = null)
        {
            await ctx.DeferAsync();
            var responsDb = await _dbContext.GetOrCreateGuild(ctx.Guild);
            if (prefix == null)
            {
                await ctx.RespondTextAsync($"The prefix of this guild is {responsDb.Prefix}");
                return;
            }

            await _dbContext.UpdatePrefix(ctx.Guild, prefix);
        }
    }
}
