using Discord;
using Discord.Interactions;

namespace GrechkaBOT.Modeles
{
    public class IntaractionModuleOthers : InteractionModuleBase<SocketInteractionContext>
    {
        public readonly InteractionService _service;

        public IntaractionModuleOthers(InteractionService service) {
            _service = service;
        }

        [SlashCommand("help", "help command")]
        public async Task HelpAsync() {
            var embed = new EmbedBuilder()
            {
                Color = Color.DarkBlue,
                Description = "**List of all available commands that are available to the bot**\n[Github project](https://github.com/SylvesterFox/GrechkaBOT-Sharp)\n This is a special version only for this server"
            };

            foreach (var module in _service.Modules) {
                string description = null;

                foreach (var cmd in module.SlashCommands) {
                    var result = cmd.Name;
                    description += $" /`{result}`";
                }

                if (!string.IsNullOrEmpty(description)) {
                    embed.AddField(x =>{
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }

            }
            embed.WithCurrentTimestamp();
            embed.WithFooter(x => {
                x.Text = "GrechakaBot (CodeName) v1.05[DEV] • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });

            await RespondAsync("", embed: embed.Build());
        }

    }
}