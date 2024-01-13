using Discord;
using Discord.Interactions;


namespace GrechkaBOT.Modeles
{
    public abstract class ZenithBase : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// Send as embed containing
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="imageUrl"></param>
        /// <param name="field"></param>
        /// <param name="author"></param>
        /// <param name="footer"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public async Task<RuntimeResult> SendEmbedAsync(
            string? title = null,
            string? description = null, 
            string? imageUrl = null,
            List<EmbedFieldBuilder>? field = null, 
            EmbedAuthorBuilder? author = null, 
            EmbedFooterBuilder? footer = null,
            Color? color = null,
            Boolean timestamp = false,
            Boolean ephemeral = false
            )
        {
            var colorEmbed = color ?? Color.Purple;

            var builder = new EmbedBuilder()
            {
                Title = title,
                Description = description,
                ImageUrl = imageUrl,
                Color = colorEmbed,
            };

            if (author != null)
            {
                builder.WithAuthor(author);
            }

            if (footer != null) 
            {
                builder.WithFooter(footer);
            } else
            {
                builder.WithFooter(x => {
                    x.Text = "ZenithBeep v1.06[DEV] • SylvesterNotCute © Все права задраконины";
                    x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
                });
            }

            if (field != null)
            {
                builder.WithFields(field);

            }
            if (timestamp)
            {
                builder.WithCurrentTimestamp();
            }

            await RespondAsync(embed: builder.Build(), ephemeral: ephemeral);

            return GrechkaResult.FromSuccess();
        }
    }
}
