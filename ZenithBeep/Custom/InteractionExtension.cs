using Discord;
using ZenithBeepData.Models;


namespace ZenithBeep.Custom
{
    public static class InteractionExtension
    {
        public static Task RespondOrFollowup(this IDiscordInteraction interaction, string text = null,
            Embed[] embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions allowed = null,
            MessageComponent component = null,
            Embed embed = null,
            RequestOptions options = null)
        {
            if (interaction.HasResponded)
            {
                
                return interaction.FollowupAsync(text, embeds, isTTS, ephemeral, allowed, component, embed, options);
            }

            return interaction.RespondAsync(text, embeds, isTTS, ephemeral, allowed, component, embed, options);
        }

        public static IEnumerable<string> GetPagedRoles(this List<ModelRoles> roles, int items)
        {
            List<string> pages = new List<string>();
            List<string> lines = new List<string>();
            int count = 0;

            foreach (ModelRoles role in roles) 
            {
                string line = $"• **IdKey:** `{role.Id}` Role: <@&{role.roleId}> - **Emoji:** {role.setEmoji}";
                count++;
                if (count % (items + 1) == 0)
                {
                    lines.Reverse();
                    pages.Add(string.Join("\n", lines));
                    lines.Clear();
                }
                lines.Add(line);

            }

            lines.Reverse();
            pages.Add(string.Join("\n", lines));
            return pages;
        }
            
    }


}
