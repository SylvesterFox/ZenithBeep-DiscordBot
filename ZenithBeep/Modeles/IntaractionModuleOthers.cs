using Discord;
using Discord.Interactions;
using ZenithBeepData;

namespace ZenithBeep.Modeles
{
    public class IntaractionModuleOthers : ZenithBase
    {
        public readonly InteractionService _service;
       

        public IntaractionModuleOthers(InteractionService service, DataAccessLayer accessLayer) : base(accessLayer) 
        {
            
            _service = service;
        }

        [SlashCommand("prefix", "Change prefix command")]
        public async Task<RuntimeResult> Prefix(string prefix = null)
        {
            await DeferAsync();
            if (prefix == null)
            {
                var currentPrefix = DataAccessLayer.GetPrefix(Context.Guild.Id);
                await FollowupAsync($"The prefix of this guild is {currentPrefix}.");
                return ZenithResult.FromSuccess();
            }

            await DataAccessLayer.SetPrefix(Context.Guild.Id, prefix);
            await FollowupAsync($"The prefix has been set to {prefix}.");
            return ZenithResult.FromSuccess();
        }


        [SlashCommand("help", "help command")]
        public async Task HelpAsync() {
            await DeferAsync();
            List<EmbedFieldBuilder> fieldBuilders = new List<EmbedFieldBuilder>();
            Color color = Color.DarkBlue;
            string title = "**List of all available commands that are available to the bot**\n[Github project](https://github.com/SylvesterFox/GrechkaBOT-Sharp)\n This is a special version only for this server";

            foreach (var module in _service.Modules)
            {
                string description = null;

                foreach (var cmd in module.SlashCommands)
                {
                    var result = cmd.Name;
                    description += $" /`{result}`";
                }

                if (!string.IsNullOrEmpty(description))
                {

                    var name = module.Name;
                    var field = new EmbedFieldBuilder()
                    {
                        Name = name.ToString(),
                        Value = description,
                        IsInline = false
                    };
                    fieldBuilders.Add(field);
                }

            }
            await SendEmbedAsync(description: title, color: color, field: fieldBuilders, timestamp: true);

        }

    }
}