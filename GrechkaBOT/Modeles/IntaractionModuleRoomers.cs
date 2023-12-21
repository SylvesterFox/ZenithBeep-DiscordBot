using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Database;

namespace GrechkaBOT.Modeles
{
    public class IntaractionModuleRoomers : InteractionModuleBase<SocketInteractionContext> 
    {
        [SlashCommand("createroomers", "Create lobby for creating private vc rooms")]
        public async Task CreateRommers() 
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
                await RespondAsync("Create lobby 🐲");
                return;
            }
            
            await RespondAsync("Lobby exists");
        }

        [SlashCommand("deletelobby", "Delete lobby")]
        public async Task DeleteLobby() 
        {
            var guildsDB = new ModelGuild { guildId = (long)Context.Guild.Id };

            ModelGuild get_guild = DatabasePost.GetGuild<ModelGuild>(guildsDB);
            var lobbyDB = new ModelRoomsLobby { guild_key = get_guild.Id };
            ModelRoomsLobby lobby = DatabasePost.GetIdChannelLobby<ModelRoomsLobby>(lobbyDB);


            if (lobby is null) {
                await RespondAsync("Lobby is null");
                return;
            }

            DatabasePost.deleteLobby(lobbyId: lobby.lobby_id);
            var channel = Context.Guild.Channels.SingleOrDefault(x => x.Id == (ulong)lobby.lobby_id);

            if (channel is null)
            {
                await RespondAsync("Channel is null");
                return;
            }

            await channel.DeleteAsync();
            await RespondAsync("Lobby is deleted");
        }

        [SlashCommand("vcname", "Change voice channel name")]
        public async Task<RuntimeResult> Vcname(string name) {

            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;


            if (channel == null) 
            {
                return GrechkaResult.FromError("Not found voice", "Please join a voice channel");
            }

            ModelTempRoom _get_rooms_temp = new ModelTempRoom 
            {
                channel_room = (long)channel.Id
            };


            ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);

        
            if (rooms == null) 
            {
               return GrechkaResult.FromError("ErrorPrivateVoice", "Room not found");
            }
            if ((ulong)rooms.id_user != user.Id) {
                return GrechkaResult.FromUserError("Not your private voice", "Sorry! No control this private voice channel");
            }

            ModelRooms _name = new ModelRooms 
            {
                name = name,
                channel_owmer = (long)user.Id
            };

            await channel.ModifyAsync(x => x.Name = name);
            DatabasePost.updateRoomName(_name);
            await RespondAsync($"Channel name update: {name}");
            return GrechkaResult.FromSuccess();

        }

        [SlashCommand("vclock", "lock/unlock private voice channel")]
        public async Task<RuntimeResult> VoiceLock() 
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;
            var EveryoneRole = Context.Guild.EveryoneRole;


            if (channel == null) 
            {
                return GrechkaResult.FromError("Not found voice", "Please join a voice channel");
            }

            ModelTempRoom _get_rooms_temp = new ModelTempRoom 
            {
                channel_room = (long)channel.Id
            };

            ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);

            if (rooms == null) {
                return GrechkaResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if ((ulong)rooms.id_user != user.Id) {
                return GrechkaResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

        
            var perEveryone = channel.GetPermissionOverwrite(EveryoneRole);
            switch (perEveryone.Value.Connect) {
                case PermValue.Allow:
                    Console.WriteLine("lock voice");
                    var _perOverides_lock = new OverwritePermissions(connect: PermValue.Deny);
                    await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides_lock);
                    await RespondAsync("Voice channel: lock");
                    break;
                case PermValue.Deny:
                    Console.WriteLine("unlock voice");
                    var _perOverides_unlock = new OverwritePermissions(connect: PermValue.Allow);
                    await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides_unlock);
                    await RespondAsync("Voice channel: unlock");
                    break;
                case PermValue.Inherit:
                    Console.WriteLine("the void");
                    var _perOverides = new OverwritePermissions(connect: PermValue.Deny);
                    await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides);
                    await RespondAsync("Voice channel: lock");
                    break;

            }
            
            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("vclimit", "Update limit private voice channel")]
        public async Task<RuntimeResult> VoiceLimit(int limit) {
            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;

            if (channel == null) 
            {
                return GrechkaResult.FromError("Not found voice", "Please join a voice channel");
            }

            ModelTempRoom _get_rooms_temp = new ModelTempRoom 
            {
                channel_room = (long)channel.Id
            };

            ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);

            if (rooms == null) {
                return GrechkaResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if ((ulong)rooms.id_user != user.Id) {
                return GrechkaResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if (limit > 100) {
                return GrechkaResult.FromUserError("OverMaxError", "Value exceeds available");
            }

            await channel.ModifyAsync(x => x.UserLimit = limit);
            await RespondAsync($"Limit change: {limit}");
            ModelRooms _limit_update = new ModelRooms 
            {
                limit = limit,
                channel_owmer = (long)user.Id
            };
            DatabasePost.updateRoomLimit(_limit_update);

            return GrechkaResult.FromSuccess();

        }           

    }
}

