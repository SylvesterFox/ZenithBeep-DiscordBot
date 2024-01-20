using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ZenithBeepData;


namespace ZenithBeep.Modeles
{
    public class IntaractionModuleRoomers : ZenithBase
    {

        public IntaractionModuleRoomers (DataAccessLayer Datalayer) : base (Datalayer)
        {

        }

        /* [SlashCommand("createroomers", "Create lobby for creating private vc rooms")]
         [DefaultMemberPermissions(GuildPermission.ManageChannels)]
         public async Task<RuntimeResult> CreateRommers() 
         {
             var guildsDB = new ModelGuild { guildId = (long)Context.Guild.Id };

             ModelGuild get_guild = DatabasePost.GetGuild<ModelGuild>(guildsDB);
             var lobbyDB = new ModelRoomsLobby { guild_key = get_guild.Id };
             ModelRoomsLobby lobby = DatabasePost.GetIdChannelLobby<ModelRoomsLobby>(lobbyDB);

             if (lobby is null) 
             {
                 var lobbyChannel = await Context.Guild.CreateVoiceChannelAsync("[+] Create voice channel [+]");
                 var insert_db = new ModelRoomsLobby 
                 {
                     guild_key = get_guild.Id,
                     lobby_id = (long)lobbyChannel.Id
                 };
                 DatabasePost.insertLobby(insert_db);
                 await SendEmbedAsync("Success!", "The lobby has been successfully created 🐲", color: Color.Green);
                 return ZenithResult.FromSuccess();
             }

             return ZenithResult.FromUserError("ErrorExists", "Lobby exists!");
         }

         [SlashCommand("deletelobby", "Delete lobby")]
         [DefaultMemberPermissions(GuildPermission.ManageChannels)]
         public async Task<RuntimeResult> DeleteLobby() 
         {
             var guildsDB = new ModelGuild { guildId = (long)Context.Guild.Id };

             ModelGuild get_guild = DatabasePost.GetGuild<ModelGuild>(guildsDB);
             var lobbyDB = new ModelRoomsLobby { guild_key = get_guild.Id };
             ModelRoomsLobby lobby = DatabasePost.GetIdChannelLobby<ModelRoomsLobby>(lobbyDB);


             if (lobby is null) {
                 return ZenithResult.FromUserError("LobbyIsNull", "Lobby does not exist");
             }

             DatabasePost.deleteLobby(lobbyId: lobby.lobby_id);
             var channel = Context.Guild.Channels.SingleOrDefault(x => x.Id == (ulong)lobby.lobby_id);

             if (channel is null)
             {
                 return ZenithResult.FromError("LobbyChannelNotFound", "Lobby channel does not exist");
             }

             await channel.DeleteAsync();

             await SendEmbedAsync("Success!", "Lobby successfully deleted!", color: Color.Green, ephemeral: true);
             return ZenithResult.FromSuccess();
         }
 */
        /*        [SlashCommand("vcname", "Change voice channel name")]
                public async Task<RuntimeResult> Vcname(string name) {

                    SocketGuildUser user = Context.User as SocketGuildUser;
                    IVoiceChannel channel = user.VoiceChannel;

                    if (channel == null) 
                    {
                        return ZenithResult.FromError("Not found voice", "Please join a voice channel");
                    }

                    ModelTempRoom _get_rooms_temp = new ModelTempRoom 
                    {
                        channel_room = (long)channel.Id
                    };


                    ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);


                    if (rooms == null) 
                    {
                       return ZenithResult.FromError("ErrorPrivateVoice", "Room not found");
                    }
                    if ((ulong)rooms.id_user != user.Id) {
                        return ZenithResult.FromUserError("Not your private voice", "Sorry! No control this private voice channel");
                    }

                    ModelRooms _name = new ModelRooms 
                    {
                        name = name,
                        channel_owmer = (long)user.Id
                    };

                    await channel.ModifyAsync(x => x.Name = name);
                    DatabasePost.updateRoomName(_name);
                    await SendEmbedAsync("Success!", $"The voice channel's name has been changed to: {name}", color: Color.Green, ephemeral: true);
                    return ZenithResult.FromSuccess();
                }*/
        /*
                [SlashCommand("vclock", "lock/unlock private voice channel")]
                public async Task<RuntimeResult> VoiceLock() 
                {
                    SocketGuildUser user = Context.User as SocketGuildUser;
                    IVoiceChannel channel = user.VoiceChannel;
                    var EveryoneRole = Context.Guild.EveryoneRole;

                    if (channel == null) 
                    {
                        return ZenithResult.FromError("Not found voice", "Please join a voice channel");
                    }

                    ModelTempRoom _get_rooms_temp = new ModelTempRoom 
                    {
                        channel_room = (long)channel.Id
                    };

                    ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);

                    if (rooms == null) {
                        return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
                    }

                    if ((ulong)rooms.id_user != user.Id) {
                        return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
                    }


                    var perEveryone = channel.GetPermissionOverwrite(EveryoneRole);
                    switch (perEveryone.Value.Connect) {
                        case PermValue.Allow:
                            var _perOverides_lock = new OverwritePermissions(connect: PermValue.Deny);
                            await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides_lock);
                            await SendEmbedAsync("Success!", "Private channel blocked", color: Color.Green, ephemeral: true);
                            break;
                        case PermValue.Deny:
                            Console.WriteLine("unlock voice");
                            var _perOverides_unlock = new OverwritePermissions(connect: PermValue.Allow);
                            await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides_unlock);
                            await SendEmbedAsync("Success!", "Private channel unlocked", color: Color.Green, ephemeral: true);
                            break;
                        case PermValue.Inherit:
                            var _perOverides = new OverwritePermissions(connect: PermValue.Deny);
                            await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides);
                            await SendEmbedAsync("Success!", "Private channel blocked", color: Color.Green, ephemeral: true);
                            break;

                    }

                    return ZenithResult.FromSuccess();
                }

                [SlashCommand("vclimit", "Update limit private voice channel")]
                public async Task<RuntimeResult> VoiceLimit(int limit) {
                    SocketGuildUser user = Context.User as SocketGuildUser;
                    IVoiceChannel channel = user.VoiceChannel;
                    var embed = new EmbedBuilder();

                    if (channel == null) 
                    {
                        return ZenithResult.FromError("Not found voice", "Please join a voice channel");
                    }

                    ModelTempRoom _get_rooms_temp = new ModelTempRoom 
                    {
                        channel_room = (long)channel.Id
                    };

                    ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);

                    if (rooms == null) {
                        return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
                    }

                    if ((ulong)rooms.id_user != user.Id) {
                        return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
                    }

                    if (limit > 100) {
                        return ZenithResult.FromUserError("OverMaxError", "Value exceeds available");
                    }

                    await channel.ModifyAsync(x => x.UserLimit = limit);
                    await SendEmbedAsync("Success!", $"Limit change: {limit}", ephemeral: true, color: Color.Green);
                    ModelRooms _limit_update = new ModelRooms 
                    {
                        limit = limit,
                        channel_owmer = (long)user.Id
                    };
                    DatabasePost.updateRoomLimit(_limit_update);

                    return ZenithResult.FromSuccess();

                }    */

    }
}

