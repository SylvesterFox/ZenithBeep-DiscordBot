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

        [SlashCommand("rolecreate", "Create role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> CreateRoleReaction(string emoji, string msgId, [Remainder] SocketTextChannel channel, IRole role)
        {
            /*var get_guilds = new ModelGuild { guildId = (long)Context.Guild.Id };*/
            var id = Convert.ToUInt64(msgId);
            var msg = await channel.GetMessageAsync(id);

            if (msg == null)
            {
                return ZenithResult.FromUserError("MassageNotFound", "Message by id was not found");
            }
/*
            ModelGuild db_guild = DatabasePost.GetGuild<ModelGuild>(get_guilds);*/
            

/*            if (Emote.TryParse(emoji, out var emote))
            {
                var insert_db = new ModelRoles
                {
                    guilds_id_KEY = db_guild.Id,
                    channelId = (long)channel.Id,
                    setEmoji = emoji,
                    roleId = (long)role.Id,
                    messageId = (long)msg.Id,
                    roleName = role.Name
                };
                await msg.AddReactionAsync(emote);
                DatabasePost.insertRoles(insert_db);
            } 
            if (Emoji.TryParse(emoji, out var emoj))
            {
                var insert_db = new ModelRoles
                {
                    guilds_id_KEY = db_guild.Id,
                    channelId = (long)channel.Id,
                    setEmoji = emoj.Name,
                    roleId = (long)role.Id,
                    messageId = (long)msg.Id,
                    roleName = role.Name
                };
                await msg.AddReactionAsync(emoj);
                DatabasePost.insertRoles(insert_db);
            }*/

            await SendEmbedAsync("Success!", $"Add role on reaction {role.Mention}", ephemeral: true, color: Color.Green);
            return ZenithResult.FromSuccess();
        }
/*
        [SlashCommand("roledelete", "Delete role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task<RuntimeResult> DeleteReactionsRole(String emoji, String mesaageid)
        {
            var get_guild = new ModelGuild { guildId = (long)Context.Guild.Id };
            ModelGuild db_guild = DatabasePost.GetGuild<ModelGuild>(get_guild);
            var id = Convert.ToUInt64(mesaageid);

            var delete_role_r = new ModelRoles
            {
                guilds_id_KEY = db_guild.Id,
                setEmoji = emoji,
                messageId = (long)id
            };
            

            ModelRoles roles = DatabasePost.GetRole<ModelRoles>(delete_role_r);
            var channel = Context.Guild.GetTextChannel((ulong)roles.channelId);
            var msg = await channel.GetMessageAsync((ulong)roles.messageId);

            if (msg == null)
            {
                return ZenithResult.FromUserError("MassageNotFound", "Message by id was not found");

            }

            String role_name = roles.roleName;
            if (Emote.TryParse(roles.setEmoji, out var emote))
            {
                await msg.RemoveAllReactionsForEmoteAsync(emote);
            }

            if (Emoji.TryParse(roles.setEmoji, out var emo))
            {
                await msg.RemoveAllReactionsForEmoteAsync(emo);
            }
            DatabasePost.deleteRoles(delete_role_r);


            await SendEmbedAsync("Success!", $"Role on reaction delete: {role_name}", ephemeral: true, color: Color.Green);
            return ZenithResult.FromSuccess();
        }*/
        
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
            var _user = user ?? Context.User;

            var author_embed = new EmbedAuthorBuilder();
            author_embed.WithName($"Photo profile: {_user.Username}");
            author_embed.WithIconUrl(Context.User.GetAvatarUrl());
            await SendEmbedAsync(description: $"Photo profile [link]({_user.GetAvatarUrl(size: 1024)})", imageUrl: _user.GetAvatarUrl(size: 1024), author: author_embed);
            return ZenithResult.FromSuccess();
        }
    }
}
