
using DSharpPlus.Entities;
using System.Drawing;

namespace ReworkZenithBeep.MessageEmbeds
{
    public static class EmbedTempalte
    {
        public static DiscordColor beepColor { get; } = new DiscordColor("#bb6ef6");


        public static DiscordEmbed UniEmbed(string title, string descripton, DiscordColor? color)
        {
            var builder = new DiscordEmbedBuilder()
            {
                Color = color ?? beepColor,
                Title = title,
            };
            return builder.Build();
        }

        private const string ErrorMessage = "Oops, something's broken";
        public static DiscordEmbed ErrorEmbed()
        {
            var builder = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = ErrorMessage,
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
