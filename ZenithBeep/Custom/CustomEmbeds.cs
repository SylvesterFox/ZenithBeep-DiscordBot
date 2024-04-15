using Discord;
using ZenithBeep.ResourcesBot;

namespace ZenithBeep.Custom
{
    public static class CustomEmbeds
    {
        public static Color success_color { get; } = Color.DarkMagenta;
        public static Color error_color { get; } = Color.Red;

        public static Color warning_color { get; } = Color.Orange;

        public static Embed UniEmbed(string title, string description = null, string iconUri = null)
        {
            var builder = new EmbedBuilder()
            {
                Color = success_color,
                Description = description ?? string.Empty
            };

            var author = new EmbedAuthorBuilder()
            {
                IconUrl = iconUri ?? "https://i.imgur.com/WxskMc9.png",
                Name = title,
                
            };

            builder.WithFooter(x => {
                x.Text = $"ZenithBeep{Others.VERSION.Replace("Branch: ", "• ")} • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });

            builder.Author = author;

            return builder.Build();
        }

        private const string shortErrorMessage = "Oops, something's broken. >.<";
        public static Embed ErrorEmbed(string errorMessage)
        {
            var builder = new EmbedBuilder()
            {
                Color = error_color,
                Description = errorMessage
            };

            var author = new EmbedAuthorBuilder()
            {
                IconUrl = "https://i.imgur.com/F8Uv2RI.png",
                Name = shortErrorMessage,

            };

            builder.WithFooter(x => {
                x.Text = $"ZenithBeep{Others.VERSION.Replace("Branch: ", "• ")} • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });

            builder.Author = author;
            return builder.Build();

        }

        public static Embed WarningEmbed(string warningMessage, string warningTitle = null)
        {
            var builder = new EmbedBuilder()
            {
                Color = warning_color,
                Description = warningMessage,
            };

            var author = new EmbedAuthorBuilder()
            {
                IconUrl = "https://i.imgur.com/cROLvqK.png",
                Name = warningTitle ?? "Warning!",

            };

            builder.WithFooter(x => {
                x.Text = $"ZenithBeep{Others.VERSION.Replace("Branch: ", "• ")} • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });

            builder.Author = author;
            return builder.Build();

        }

    }
}
