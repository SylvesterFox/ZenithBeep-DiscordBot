
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using ReworkZenithBeep.Module.Music;
using ReworkZenithBeep.Settings;


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

        public PaginationMessage(IEnumerable<DiscordEmbedBuilder> builders, string title = "", string embedColor = "#2C2F33", DiscordMember user = null, AppearanceOptions options = null)
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
        private static PaginationService instance;

        public PaginationService(DiscordClient client)
        {
            messages = new Dictionary<ulong, PaginationMessage>();
            client.ComponentInteractionCreated += ButtonHandler;
        }

        public static PaginationService GetInstance(DiscordClient discordClient)
        {
            if (instance == null)
            {
                instance = new PaginationService(discordClient);
            }
            return instance;
        }

        public async Task ButtonHandler(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            if (messages.TryGetValue(args.Message.Id, out PaginationMessage page))
            {
                if (args.User.Id != page.User.Id)
                {
                    return;
                }

                switch (args.Interaction.Data.CustomId)
                {
                    case "first":
                        if (page.CurrentPage != 1)
                        {
                            page.CurrentPage = 1;
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(page.GetEmbed()).AddComponents(args.Message.Components));
                        }
                        break;

                    case "back":
                        if (page.CurrentPage != 1)
                        {
                            page.CurrentPage--;
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(page.GetEmbed()).AddComponents(args.Message.Components));
                        }
                        break;

                    case "next":
                        if (page.CurrentPage != page.Count)
                        {
                            page.CurrentPage++;
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(page.GetEmbed()).AddComponents(args.Message.Components));
                        }
                        break;

                    case "last":
                        if (page.CurrentPage != page.Count)
                        {
                            page.CurrentPage = page.Count;
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(page.GetEmbed()).AddComponents(args.Message.Components));
                        }
                        break;

                    case "stop":
                        switch (page.Options.OnStop)
                        {
                            case StopAction.DeleteMessage:
                                await args.Message.DeleteAsync();
                                break;

                            case StopAction.Clear:
                                await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                                    .WithContent("No more buttons for you"));
                                await Task.Delay(1000);
                                await args.Message.DeleteAsync();
                                break;
                        }

                        messages.Remove(args.Message.Id);
                        break;

                    case "select":
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        messages.Remove(args.Message.Id);
                        break;
                }
            }
        }

        public async Task<DiscordMessage> SendMessageAsync(CommonContext ctx, PaginationMessage pagination, bool folloup = false)
        {
            DiscordMessage message;
            var builder = new DiscordMessageBuilder();

            if (pagination.Count > 1)
            {
                
                switch (pagination.Options.Style) 
                {
                    case DisplayStyle.Full:
                        builder.AddComponents(new DiscordComponent[]
                        {
                            new DiscordButtonComponent(ButtonStyle.Secondary, "first", "First"),
                            new DiscordButtonComponent(ButtonStyle.Primary, "back", "Back"),
                            new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next"),
                            new DiscordButtonComponent(ButtonStyle.Secondary, "last", "Last")
                        });
                        break;

                    case DisplayStyle.Minimal:
                       builder.AddComponents(new DiscordComponent[]
                       {
                            new DiscordButtonComponent(ButtonStyle.Primary, "back", "Back"),
                            new DiscordButtonComponent(ButtonStyle.Danger, "stop", "X"),
                            new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next"),                      
                       });
                       break;

                    case DisplayStyle.Selector:
                        builder.AddComponents(new DiscordComponent[]
                        {
                            new DiscordButtonComponent(ButtonStyle.Primary, "back", "Back"),
                            new DiscordButtonComponent(ButtonStyle.Success, "select", "Select"),
                            new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next"),
                        });
                        break;

                }

                if (folloup)
                {
                    await ctx.DeferAsync();
                    await ctx.RespondAsync(builder.AddEmbed(pagination.GetEmbed()));
                    message = await ctx.GetOriginalResponseAsync();
                }
                else
                {
                    await ctx.RespondAsync(builder.AddEmbed(pagination.GetEmbed()));
                    message = await ctx.GetOriginalResponseAsync();
                }

            } else
            {
                if (folloup) 
                {
                    await ctx.DeferAsync();
                    await ctx.RespondAsync(new DiscordMessageBuilder().AddEmbed(pagination.GetEmbed()));
                    message = await ctx.GetOriginalResponseAsync();
                } else
                {
                    await ctx.RespondAsync(new DiscordMessageBuilder().AddEmbed(pagination.GetEmbed()));
                    message = await ctx.GetOriginalResponseAsync();
                }

                return message;
            }

            messages.Add(message.Id, pagination);

            if (pagination.Options.Timeout != TimeSpan.Zero)
            {
                Task _ = Task.Delay(pagination.Options.Timeout).ContinueWith(async t =>
                {
                    if (!messages.ContainsKey(message.Id))
                    {
                        return;
                    }

                    switch (pagination.Options.TimeoutAction)
                    {
                        case StopAction.DeleteMessage:
                            await message.DeleteAsync();
                            break;
                        case StopAction.Clear:
                            await message.DeleteAllReactionsAsync(); 
                            break;
                    }

                    messages.Remove(message.Id);
                });
            }

            return message;

        }
    }
}
