

using DSharpPlus;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.MessageEmbeds;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.RolesGet
{
    public partial class RoleSelectors
    {
        private static RoleSelectors instance;
        private readonly DataBot _dbContext;
        public RoleSelectors(DataBot dbContext)
        {
            _dbContext = dbContext;
        }

        public static RoleSelectors GetInstance(DataBot dbContext)
        {
            if (instance == null)
            {
                instance = new RoleSelectors(dbContext);
            }
            return instance;
        }

        private DiscordEmoji? _GetEmote(InteractionContext ctx, string emote)
        {
          
            if (DiscordEmoji.TryFromUnicode(emote, out var result))
            {
                return result;
            }

            IEmojiParser? emojiParser = new EmojiParser(emote);

            if (DiscordEmoji.TryFromGuildEmote(ctx.Client, emojiParser.GetId(), out var emoji1))
            {
                return emoji1;
            }
         
            return null;
        }

        public async Task CreateRolesCommand(InteractionContext ctx, DiscordChannel? channel, DiscordRole role, ulong msgId , string emoji)
        {
            /*await ctx.DeferAsync(true);*/
            DiscordMessage? _msg = await ctx.Channel.GetMessageAsync(msgId);
            DiscordChannel _channel = channel ?? ctx.Channel;
            if (channel != null)
            {
                _msg = await channel.GetMessageAsync(msgId);
            }

            if (_msg == null)
            {
                var embedError = EmbedTempalte.ErrorEmbed("Message by id was not found! >~<", "MassageNotFound");
                await ctx.CreateResponseAsync(embedError);
                return;
            }

            var _emoji = _GetEmote(ctx, emoji); 
            if (_emoji != null)
            {
                bool success = await _dbContext.CreateRolesSelector(ctx.Guild, _msg.Id, role.Id, _channel.Id, _emoji.Name);
                if (success == false)
                {
                    var embedError = EmbedTempalte.ErrorEmbed("This object already exists! >~<", "DataObjectExists");
                    await ctx.CreateResponseAsync(embedError);
                    return;
                }

                await _msg.CreateReactionAsync(_emoji);
                var embedSuccess = new EmbedTempalte.DetailedEmbedContent {
                    Color = new DiscordColor("#72f963"),
                    Description = $"Add role on reaction {role.Mention}",
                    Title = "Success!"
                };
                var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
                await ctx.CreateResponseAsync(embed);
            }

        }
    }
}
