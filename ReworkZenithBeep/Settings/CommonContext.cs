
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ReworkZenithBeep.Settings
{
    public abstract class CommonContext
    {
        public abstract DiscordGuild Guild { get; }
        public abstract DiscordMember? Member { get; }
        public abstract DiscordChannel Channel { get; }
        public bool IsDefered { get; protected set; } = false;
        public bool IsResponsed { get; protected set; } = false;
        public DiscordMessage? RespondMessage { get; protected set; }

        public virtual async Task DeferAsync(bool ephemeral = false) { IsDefered = true; }
        public abstract Task RespondAsync(DiscordMessageBuilder messageBuilder);
        public virtual async Task RespondEmbedAsync(DiscordEmbed embed, bool ephemeral = false, DiscordComponent[]? components = null)
        {
            var msg = new DiscordMessageBuilder().AddEmbed(embed);
            if (components != null)
            {
                msg.AddComponents(components);
            }
            await RespondAsync(msg);
        }
        public virtual async Task RespondTextAsync(string content, bool ephemeral = false)
        {
            var msg = new DiscordMessageBuilder().WithContent(content);
            await RespondAsync(msg);
        }

        public virtual async Task EditResponseAsync(DiscordMessageBuilder messageBuilder, IEnumerable<DiscordAttachment> attachments = null)
        {
            if (RespondMessage is null)
            {
                await RespondAsync(messageBuilder);
                return;
            }
        }

    }

    public class NextCommand(CommandContext commandContext) : CommonContext
    {
        public override DiscordGuild Guild => commamdContext.Guild;
        public override DiscordMember? Member => commamdContext.Member;
        public override DiscordChannel Channel => commamdContext.Channel;

        private readonly CommandContext commamdContext = commandContext;

        public override async Task RespondAsync(DiscordMessageBuilder messageBuilder)
        {
            RespondMessage = await commamdContext.RespondAsync(messageBuilder);
            IsResponsed = true;
        }

        public override async Task EditResponseAsync(DiscordMessageBuilder messageBuilder, IEnumerable<DiscordAttachment> attachments = null)
        {
            await base.EditResponseAsync(messageBuilder);
            await RespondMessage.ModifyAsync(messageBuilder);
        }
    }

    public class SlashContext(InteractionContext interactionContext) : CommonContext
    {
        public override DiscordGuild Guild => interactionContext.Guild;
        public override DiscordMember? Member => interactionContext.Member;
        public override DiscordChannel Channel => interactionContext.Channel;

        private readonly InteractionContext interactionContext = interactionContext;

        public override async Task DeferAsync(bool ephemeral)
        {
            await base.DeferAsync(ephemeral);
            await interactionContext.DeferAsync(ephemeral);
        }

        public override async Task RespondAsync(DiscordMessageBuilder messageBuilder)
        {
            if (!IsDefered)
            {
                if (!IsResponsed)
                {
                    IsResponsed = true;
                    await interactionContext.CreateResponseAsync(new DiscordInteractionResponseBuilder(messageBuilder));
                    return;
                }
                IsResponsed = true;
                await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder(messageBuilder));
                return;
            }
            IsDefered = false;
            IsResponsed = true;
            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder(messageBuilder));
            
        }
        

    }

}
