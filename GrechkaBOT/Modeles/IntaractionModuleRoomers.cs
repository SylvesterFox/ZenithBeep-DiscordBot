using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Database;

namespace GrechkaBOT.Modeles
{
    public class IntaractionModuleRoomers : InteractionModuleBase<SocketInteractionContext> 
    {
        [SlashCommand("createroomers", "Create lobby for creating private vc rooms")]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        public async Task<RuntimeResult> CreateRommers() 
        {
            var embed = new EmbedBuilder();
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
                embed.WithColor(Color.Green);
                embed.WithTitle("Success!");
                embed.WithDescription("The lobby has been successfully created 🐲");
                DatabasePost.insertLobby(insert_db);
                await RespondAsync("", embed: embed.Build(), ephemeral: true);
                return GrechkaResult.FromSuccess();
            }
            
            return GrechkaResult.FromUserError("ErrorExists", "Lobby exists!");
        }

        [SlashCommand("deletelobby", "Delete lobby")]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        public async Task<RuntimeResult> DeleteLobby() 
        {
            var embed = new EmbedBuilder();
            var guildsDB = new ModelGuild { guildId = (long)Context.Guild.Id };

            ModelGuild get_guild = DatabasePost.GetGuild<ModelGuild>(guildsDB);
            var lobbyDB = new ModelRoomsLobby { guild_key = get_guild.Id };
            ModelRoomsLobby lobby = DatabasePost.GetIdChannelLobby<ModelRoomsLobby>(lobbyDB);


            if (lobby is null) {
                return GrechkaResult.FromUserError("LobbyIsNull", "Lobby does not exist");
            }

            DatabasePost.deleteLobby(lobbyId: lobby.lobby_id);
            var channel = Context.Guild.Channels.SingleOrDefault(x => x.Id == (ulong)lobby.lobby_id);

            if (channel is null)
            {
                return GrechkaResult.FromError("LobbyChannelNotFound", "Lobby channel does not exist");
            }

            await channel.DeleteAsync();
            embed.WithColor(Color.Green);
            embed.WithTitle("Success!");
            embed.WithDescription("Lobby successfully deleted");
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("vcname", "Change voice channel name")]
        public async Task<RuntimeResult> Vcname(string name) {

            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Green);
            embed.WithTitle("Success!");

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
            embed.WithDescription($"The voice channel's name has been changed to: {name}");
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("vclock", "lock/unlock private voice channel")]
        public async Task<RuntimeResult> VoiceLock() 
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;
            var EveryoneRole = Context.Guild.EveryoneRole;
            
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Green);
            embed.WithTitle("Success!");

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
                    var _perOverides_lock = new OverwritePermissions(connect: PermValue.Deny);
                    await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides_lock);
                    embed.WithDescription("Private channel blocked");
                    await RespondAsync("", embed: embed.Build(), ephemeral: true);
                    break;
                case PermValue.Deny:
                    Console.WriteLine("unlock voice");
                    var _perOverides_unlock = new OverwritePermissions(connect: PermValue.Allow);
                    await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides_unlock);
                    embed.WithDescription("Private channel unlocked");
                    await RespondAsync("", embed: embed.Build(), ephemeral: true);
                    break;
                case PermValue.Inherit:
                    var _perOverides = new OverwritePermissions(connect: PermValue.Deny);
                    await channel.AddPermissionOverwriteAsync(EveryoneRole, _perOverides);
                    embed.WithDescription("Private channel blocked");
                    await RespondAsync("", embed: embed.Build(), ephemeral: true);
                    break;

            }
            
            return GrechkaResult.FromSuccess();
        }

        [SlashCommand("vclimit", "Update limit private voice channel")]
        public async Task<RuntimeResult> VoiceLimit(int limit) {
            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Green);
            embed.WithTitle("Success!");

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
            embed.WithDescription($"Limit change: {limit}");
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
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

