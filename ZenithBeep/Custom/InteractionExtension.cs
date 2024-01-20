using Discord;


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
            
    }
}
