using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Database;


namespace GrechkaBOT.Modeles
{
    public class IntaractionModuleUtility : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("rolecreate", "Create role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task CreateRoleReaction(string emoji, string msgId, [Remainder] SocketTextChannel channel, IRole role)
        {
            var get_guilds = new ModelGuild { guildId = (long)Context.Guild.Id };
            var id = Convert.ToUInt64(msgId);
            // TODO: Тут тоже отлов нужен
            var msg = await channel.GetMessageAsync(id);

            if (msg == null)
            {
                await RespondAsync("Message by id was not found", ephemeral: true);
                return;

            }

            ModelGuild db_guild = DatabasePost.GetGuild<ModelGuild>(get_guilds);
            

            if (Emote.TryParse(emoji, out var emote))
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
            }

            await RespondAsync($"Add role on reaction {role.Mention}", ephemeral: true); 
            
            
        }

        [SlashCommand("roledelete", "Delete role")]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task DeleteReactionsRole(String emoji, String mesaageid)
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
            //TODO: Сделать тут отлов msg исключения
            var msg = await channel.GetMessageAsync((ulong)roles.messageId);

            if (msg == null)
            {
                await RespondAsync("Message by id was not found", ephemeral: true);
                return;

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


            await RespondAsync($"Role on reaction delete: {role_name}", ephemeral: true);
        }
    }
}
