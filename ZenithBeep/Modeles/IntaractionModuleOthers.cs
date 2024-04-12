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
        public async Task<RuntimeResult> Prefix(string? prefix = null)
        {
            await DeferAsync();
            var guildDb = await DataAccessLayer.GetOrCreateGuild(Context.Guild);
            if (prefix == null)
            {
                var currentPrefix = await DataAccessLayer.GetPrefix(guildDb.Id);
                await FollowupAsync($"The prefix of this guild is {currentPrefix}.");
                return ZenithResult.FromSuccess();
            }

            await DataAccessLayer.SetPrefix(guildDb.Id, prefix);
            await FollowupAsync($"The prefix has been set to {prefix}.");
            return ZenithResult.FromSuccess();
        }


        [SlashCommand("help", "help command")]

        public async Task HelpAsync() {
            await DeferAsync();
            List<EmbedFieldBuilder> fieldBuilders = new List<EmbedFieldBuilder>();
            Color color = Color.DarkBlue;
            string title = "**List of all available commands that are available to the bot**\n[Github project](https://github.com/SylvesterFox/ZenithBeep-DiscordBot)\n This is a special version only for this server";
     

            foreach (var module in _service.Modules)
            {
                string description = String.Empty;

                foreach (var cmd in module.SlashCommands)
                {
                    var result = cmd.Name;
                    description += $" /`{result}`";
                }

                if (!string.IsNullOrEmpty(description))
                {
                    string name = String.Empty;
                    try
                    {
                      name = ResourcesBot.Help_en.ResourceManager.GetString(module.Name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    if (name is null)
                    {
                        name = module.Name;
                    }


                    var field = new EmbedFieldBuilder()
                    {
                        Name = name,
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