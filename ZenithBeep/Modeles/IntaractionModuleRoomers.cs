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

        [SlashCommand("createlobby", "Create lobby for creating private vc rooms")]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        public async Task<RuntimeResult> CreateRommers()
        {
            await DeferAsync();
            var guild_db = await DataAccessLayer.GetOrCreateGuild(Context.Guild);
            var lobby = await DataAccessLayer.dataRooms.GetLobby(guild_db.Id);
            if (lobby != null)
            {
                return ZenithResult.FromUserError("ErrorExists", "Lobby exists!");
            }

            var newlobbyChannel = await Context.Guild.CreateVoiceChannelAsync("[+] Create voice channel [+]");
            await DataAccessLayer.dataRooms.CreateLobby(guild_db.Id, newlobbyChannel.Id);
            await SendEmbedAsync("Success!", "The lobby has been successfully created 🐲", color: Color.Green);
            return ZenithResult.FromSuccess();
        }

        [SlashCommand("deletelobby", "Delete lobby")]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        public async Task<RuntimeResult> DeleteLobby()
        {
            await DeferAsync();
            var lobby = await DataAccessLayer.dataRooms.GetLobby(Context.Guild.Id);

            if (lobby is null)
            {
                return ZenithResult.FromUserError("LobbyIsNull", "Lobby does not exist");
            }

            await DataAccessLayer.dataRooms.DeleteLobby(lobby.lobby_id);

            var channel = Context.Guild.Channels.SingleOrDefault(x => x.Id == lobby.lobby_id);

            if (channel is null)
            {
                return ZenithResult.FromError("LobbyChannelNotFound", "Lobby channel does not exist");
            }

            await channel.DeleteAsync();

            await SendEmbedAsync("Success!", "Lobby successfully deleted!", color: Color.Green, ephemeral: true);
            return ZenithResult.FromSuccess();
        }



        [SlashCommand("vclock", "lock/unlock private voice channel")]
        public async Task<RuntimeResult> VoiceLock()
        {
            await DeferAsync(ephemeral: true);
            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;
            var EveryoneRole = Context.Guild.EveryoneRole;

            if (channel == null)
            {
                return ZenithResult.FromError("Not found voice", "Please join a voice channel");
            }

            var rooms = await DataAccessLayer.dataRooms.GetTempRoom(channel.Id);

            if (rooms == null)
            {
                return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if (rooms.userId != user.Id)
            {
                return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }


            var perEveryone = channel.GetPermissionOverwrite(EveryoneRole);
            switch (perEveryone.Value.Connect)
            {
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

        [SlashCommand("limit", "Update limit private voice channel")]
        public async Task<RuntimeResult> VoiceLimit(int limit)
        {
            await DeferAsync(ephemeral: true);
            SocketGuildUser user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;
            var embed = new EmbedBuilder();

            if (channel == null)
            {
                return ZenithResult.FromError("Not found voice", "Please join a voice channel");
            }

            var rooms = await DataAccessLayer.dataRooms.GetTempRoom(channel.Id);

            if (rooms == null)
            {
                return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if (rooms.userId != user.Id)
            {
                return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if (limit > 100)
            {
                return ZenithResult.FromUserError("OverMaxError", "Value exceeds available");
            }

            await channel.ModifyAsync(x => x.UserLimit = limit);
            await DataAccessLayer.dataRooms.SetVoicelimit(limit, rooms.userId);
            await SendEmbedAsync("Success!", $"Limit change: {limit}", ephemeral: true, color: Color.Green);
            

            return ZenithResult.FromSuccess();

        }

        [SlashCommand("change-room-name", "This command changes the name of the private channel.")]
        public async Task<RuntimeResult> ChangeName(string name)
        {
            await DeferAsync(ephemeral: true);
            SocketGuildUser? user = Context.User as SocketGuildUser;
            IVoiceChannel channel = user.VoiceChannel;

            if (channel == null)
            {
                return ZenithResult.FromError("Not found voice", "Please join a voice channel");
            }

            var rooms = await DataAccessLayer.dataRooms.GetTempRoom(channel.Id);

            if (rooms == null)
            {
                return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            if (rooms.userId != user.Id)
            {
                return ZenithResult.FromUserError("Not your private channel", "Sorry! No control this private voice channel");
            }

            await channel.ModifyAsync(x => x.Name = name);
            await DataAccessLayer.dataRooms.SetVoiceName(name, user.Id);
            await FollowupAsync($"The name of the private channel has been changed to: {name}");
            return ZenithResult.FromSuccess();
        }

    }
}

