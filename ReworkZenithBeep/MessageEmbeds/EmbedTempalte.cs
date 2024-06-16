
using DSharpPlus.Entities;

namespace ReworkZenithBeep.MessageEmbeds
{
    public static class EmbedTempalte
    {
        public static DiscordColor beepColor { get; } = new DiscordColor("#bb6ef6");


        public static DiscordEmbed UniEmbed(string title, string? color = null)
        {
            DiscordColor colorEmbed;
            if (string.IsNullOrEmpty(color))
            {
                colorEmbed = beepColor;
            } else
            {
                colorEmbed= new DiscordColor(color);
            }

            var builder = new DiscordEmbedBuilder()
            {
                Color = colorEmbed,
                Title = title,
            };
            return builder.Build();
        }

        private const string ErrorTitle = "Oops, something's broken";
        public static DiscordEmbed ErrorEmbed(string? message = null, string? title = null)
        {
            var builder = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = title ?? ErrorTitle,
                Description = message ?? string.Empty,
            };
            return builder.Build();
        }

        public static DiscordEmbed DetaliedEmbed(DetailedEmbedContent content)
        {
            if (content.GetHashCode() == DetailedEmbedContent.Empty.GetHashCode())
                throw new ArgumentException("At least one of the arguments must not be null");

            var builder = new DiscordEmbedBuilder()
            {
                Color = content.Color ?? beepColor,
                Title = content.Title,
                Description = content.Description,
            }.WithFooter(text: content.Footer);
            return builder.Build();
        }

        public struct DetailedEmbedContent
        {
            public string? Title { get; init; }
            public DiscordColor? Color { get; init; }
            public string? Description { get; init; }
            public string? Footer { get; init; }

            public static DetailedEmbedContent Empty { get; } = new();
        }
    }
}
