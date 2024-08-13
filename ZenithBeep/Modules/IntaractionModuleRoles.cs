using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Serilog;
using ZenithBeep.Custom;
using ZenithBeep.Services;
using ZenithBeepData;

namespace ZenithBeep.Modules
{
    public class IntaractionModuleRoles : ZenithBase
    {
        public PaginationService Pagination { private get; set; }
        private readonly ParseEmoji _parseEmoji;
        public IntaractionModuleRoles(DataAccessLayer dataAccessLayer, ParseEmoji parseEmoji) : base(dataAccessLayer) 
        {
            _parseEmoji = parseEmoji;
        }

        [SlashCommand("rolecreate", "Create role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> CreateRoleReaction(string emoji, string msgId, IRole role, SocketTextChannel channel = null)
        {
            await DeferAsync(ephemeral: true);
            var id = Convert.ToUInt64(msgId);

            IMessage? _msg = await Context.Channel.GetMessageAsync(id);

            if (channel != null)
            {
                _msg = await channel.GetMessageAsync(id);

            }


            var guildId = Context.Guild.Id;

            if (_msg == null)
            {
                return ZenithResult.FromUserError("MassageNotFound", "Message by id was not found");
            }

            var _emoji = _parseEmoji.GetParseEmoji(emoji);
            if (_emoji == null)
            {
                return ZenithResult.FromSuccess("IsNotEmoji", "Not emoji");
            }

            try
            {

                string name = _parseEmoji.GetNameEmoji(emoji);
                await DataAccessLayer.SetRolesAutoMod(guildId, _msg.Id, role.Id, _msg.Channel.Id, name);
                await _msg.AddReactionAsync(_emoji);

                await SendEmbedAsync("Success!", $"Add role on reaction {role.Mention}", ephemeral: true, color: Color.Green);
                return ZenithResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ZenithResult.FromError($"{ex.Source}", $"{ex.Message}");
            }

        }

        [SlashCommand("list-message-roles", "Get list roles of message")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> GetAsyncRolesMessageRoles(String messageId)
        {
            await DeferAsync(ephemeral: true);
            var msg_id = Convert.ToUInt64(messageId);
            var guildId = Context.Guild.Id;
            try
            {
                var roles = await DataAccessLayer.GetAllRoloesOfMessage(guildId, msg_id);
                List<EmbedBuilder> builders = InteractionExtension.GetPagedRoles(roles, 10).Select(str => new EmbedBuilder().WithDescription(str)).ToList();

                await Pagination.SendMeesgeAsync(Context, new PaginationMessage(builders, "All roles associated with this message.", Color.Blue, Context.User, new AppearanceOptions()
                {
                    Timeout = TimeSpan.FromMinutes(5),
                    Style = DisplayStyle.Full
                }), folloup: true);

                return ZenithResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ZenithResult.FromUserError($"{ex.GetType().Name}", $"{ex.Message}");
            }

        }

        [SlashCommand("roledelete", "Delete role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> DeleteReactionsRole(int key)
        {
            await DeferAsync(ephemeral: true);

            var guildId = Context.Guild.Id;
            var role = await DataAccessLayer.GetKeyRole(guildId, key);


            if (role == null)
            {
                return ZenithResult.FromUserError("RoleNotFound", "Role by id was not found");
            }

            var channel = Context.Guild.GetTextChannel(role.channelId);
            var msg = await channel.GetMessageAsync(role.messageId);

            if (msg == null)
            {
                return ZenithResult.FromUserError("MassageNotFound", "Message by id was not found");
            }

            try
            {
                var _emoji = _parseEmoji.GetParseEmoji(role.setEmoji);
                if (_emoji == null)
                {
                    return ZenithResult.FromSuccess("IsNotEmoji", "Not emoji");
                }

                if (_emoji != null)
                {
                    await msg.RemoveAllReactionsForEmoteAsync(_emoji);
                }

                await DataAccessLayer.DeleteRolesAutoMod(guildId, role.Id);
                await SendEmbedAsync("Success!", $"Role on reaction delete: <@&{role.roleId}>", ephemeral: true, color: Color.Green);
                return ZenithResult.FromSuccess();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Source}:{ex.Message}");
                return ZenithResult.FromError($"{ex.Source}", $"{ex.Message}");
            }

        }
    }
}
