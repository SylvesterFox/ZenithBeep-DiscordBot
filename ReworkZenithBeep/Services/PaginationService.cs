
using DSharpPlus;
using DSharpPlus.Entities;
using ReworkZenithBeep.Settings;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace ReworkZenithBeep.Services
{
    public enum StopAction
    {
        Clear,
        DeleteMessage
    }
    public enum DisplayStyle
    {
        Full,
        Minimal,
        Selector
    }

    public class PaginationMessage
    {
        private string Title { get; }
        private DiscordColor EmbedColor { get; }
        private IReadOnlyCollection<DiscordEmbed> Pages { get; }
        internal DiscordUser User { get; }
        internal AppearanceOptions Options { get; }
        internal int CurrentPage { get; set; }

        internal int Count => Pages.Count;

        public PaginationMessage(IEnumerable<DiscordEmbedBuilder> builders, string title = "", string embedColor = "#2C2F33", DiscordUser user = null, AppearanceOptions options = null)
        {
            List<DiscordEmbed> embeds = new List<DiscordEmbed>();

            int i = 1;

            foreach (DiscordEmbedBuilder embed in builders)
            {
                embed.Title ??= title;
                embed.Color = new DiscordColor(embedColor);
                embed.WithFooter(text: $"Page {i++}/{builders.Count()}");
                embeds.Add(embed.Build());
                
            }

            Pages = embeds;
            Title = title;
            EmbedColor = new DiscordColor(embedColor);
            User = user;
            Options = options ?? new AppearanceOptions();
            CurrentPage = 1;
        }

        internal DiscordEmbed GetEmbed()
        {
            return Pages.ElementAtOrDefault(CurrentPage - 1);
        }

     
    }

    public class AppearanceOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
        public DisplayStyle Style { get; set; } = DisplayStyle.Full;

        public StopAction OnStop { get; set; } = StopAction.Clear;

        public StopAction TimeoutAction { get; set; } = StopAction.Clear;
    }

    public class PaginationService
    {
        private readonly Dictionary<ulong, PaginationMessage> messages;

        public PaginationService(DiscordClient client)
        {
            messages = new Dictionary<ulong, PaginationMessage>();
 /*           client.ComponentInteractionCreated += ButtonHandler;*/
        }

  /*      public async Task<DiscordMessage> SendMessageAsync(CommonContext ctx, PaginationMessage pagination, bool folloup = false)
        {
            DiscordMessage message;

            if (pagination.Count > 1)
            {
                var builder = new DiscordComponent[]
                
            }
        }*/
    }
}
