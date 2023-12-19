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

            ModelRooms _get_rooms_settings = new ModelRooms {
                channel_owmer = (long)user.Id
            };

            ModelRooms rooms_settings = DatabasePost.GetRoom<ModelRooms>(_get_rooms_settings);
            ModelTempRoom rooms = DatabasePost.GetTempRoom<ModelTempRoom>(_get_rooms_temp);

            if (rooms != null) 
            {
                ModelRooms _name = new ModelRooms 
                {
                    name = name,
                    channel_owmer = rooms_settings.channel_owmer
                };
                var channel_voice = Context.Guild.Channels.SingleOrDefault(x => x.Id == (ulong)rooms.channel_room);
                await channel_voice.ModifyAsync(x => x.Name = name);
                DatabasePost.updateRoomName(_name);
                await RespondAsync("Channel name update");
                return GrechkaResult.FromSuccess();
            }
            return GrechkaResult.FromError("ErrorPrivateVoice", "Invaid rooms");
        }
    }
}