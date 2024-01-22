using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using ZenithBeepData;
using RuntimeResult = Discord.Interactions.RuntimeResult;


namespace ZenithBeep.Modeles
{
    public class IntaractionModuleUtility : ZenithBase
    {
        public IntaractionModuleUtility(DataAccessLayer accessLayer) : base(accessLayer) 
        {
            
        }

        public Emoji GetEmoji(string emoji)
        {
            if (Emoji.TryParse(emoji, out var result))
            {
                return result;
            } else
            {
                return null;
            }

            
        }

        public Emote GetEmote(string emote) { 
            if (Emote.TryParse(emote, out var result))
            {
                return result;
            }
            else
            {
                return null;
            }
            
        }

        [SlashCommand("rolecreate", "Create role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> CreateRoleReaction(string emoji, string msgId, [Remainder] SocketTextChannel channel, IRole role)
        {
            await DeferAsync();
            var id = Convert.ToUInt64(msgId);
            var msg = await channel.GetMessageAsync(id);
            var guildId = Context.Guild.Id;

            if (msg == null)
            {
                return ZenithResult.FromUserError("MassageNotFound", "Message by id was not found");
            }

            Emote _emote = GetEmote(emoji);
            Emoji _emoji = GetEmoji(emoji);

            try
            {
                if (_emoji != null)
                {
                    await DataAccessLayer.SetRolesAutoMod(guildId, msg.Id, role.Id, channel.Id, _emoji.Name);
                    await msg.AddReactionAsync(_emoji);
                }

                if (_emote != null)
                {
                    await DataAccessLayer.SetRolesAutoMod(guildId, msg.Id, role.Id, channel.Id, $"<:{_emote.Name}:{_emote.Id}>");
                    await msg.AddReactionAsync(_emote);
                }

                await SendEmbedAsync("Success!", $"Add role on reaction {role.Mention}", ephemeral: true, color: Color.Green);
                return ZenithResult.FromSuccess();
            } 
            catch (Exception ex)
            {
                return ZenithResult.FromError($"{ex.Source}", $"{ex.Message}");
            }

        }

        [SlashCommand("roledelete", "Delete role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> DeleteReactionsRole(String emoji, String mesaageid)
        {
            await DeferAsync();
            Console.WriteLine(emoji);
            var msg_id = Convert.ToUInt64(mesaageid);
            var guildId = Context.Guild.Id;
            var role = await DataAccessLayer.GetRoleAutoMod(guildId, msg_id, emoji);
            Emote _emote = GetEmote(emoji);
            Emoji _emoji = GetEmoji(emoji);

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
                if (_emoji != null)
                {
                    await msg.RemoveAllReactionsForEmoteAsync(_emoji);
                }

                if (_emote != null)
                {
                    await msg.RemoveAllReactionsForEmoteAsync(_emote);
                }

                await DataAccessLayer.DeleteRolesAutoMod(guildId, role.roleId, msg_id);
                await SendEmbedAsync("Success!", $"Role on reaction delete: <@&{role.roleId}>", ephemeral: true, color: Color.Green);
                return ZenithResult.FromSuccess();
            } catch (Exception ex)
            {
                Log.Error($"{ex.Source}:{ex.Message}");
                return ZenithResult.FromError($"{ex.Source}", $"{ex.Message}");
            } 
           
        }

        [SlashCommand("beep", "Ping command")]
        public async Task<RuntimeResult> PingCommand() {
           await RespondAsync("boop!! :ping_pong:");
           var msg = await GetOriginalResponseAsync();
           await msg.ModifyAsync(msg => msg.Content = $"pong.. :ping_pong: \n ping: {Context.Client.Latency}ms");
           Log.Debug("test");
           return ZenithResult.FromSuccess();
        }

        [SlashCommand("avatar", "Get user avatar")]
        public async Task<RuntimeResult> UserAvatar(SocketUser? user = null)
        {
            await DeferAsync();
            var _user = user ?? Context.User;

            var author_embed = new EmbedAuthorBuilder();
            author_embed.WithName($"Photo profile: {_user.Username}");
            author_embed.WithIconUrl(Context.User.GetAvatarUrl());
            await SendEmbedAsync(description: $"Photo profile [link]({_user.GetAvatarUrl(size: 1024)})", imageUrl: _user.GetAvatarUrl(size: 1024), author: author_embed);
            return ZenithResult.FromSuccess();
        }
    }
}
