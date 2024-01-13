using Discord;
using Discord.Interactions;

namespace GrechkaBOT.Modeles
{
    public class IntaractionModuleOthers : ZenithBase
    {
        public readonly InteractionService _service;


        public IntaractionModuleOthers(InteractionService service) {
            _service = service;
        }

        [SlashCommand("help", "help command")]
        public async Task HelpAsync() {
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
                    var name = Resources.Help_en.ResourceManager.GetObject(module.Name);
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