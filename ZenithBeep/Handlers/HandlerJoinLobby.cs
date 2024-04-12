using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using ZenithBeepData;


namespace ZenithBeep.Handlers
{
    public class HandlerJoinLobby
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _service;
        private readonly DataAccessLayer _dataAccessLayer;

        public HandlerJoinLobby(DiscordSocketClient client, IServiceProvider serviceProvider, DataAccessLayer dataAccessLayer) 
        {
            _client = client;
            _service = serviceProvider;
            _dataAccessLayer = dataAccessLayer;
        }


        public async Task InitializeAsync() 
        {
            _client.UserVoiceStateUpdated += OnCreatePrivateRoom;
        }


        private async Task OnCreatePrivateRoom(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            if (user.IsBot)
                return;

            // Выполняется при заходи в лоби по созданию приватного войса
            if (state2.VoiceChannel != null)
            {
                SocketGuild _guild = state2.VoiceChannel.Guild;
                var guild_db = await _dataAccessLayer.GetOrCreateGuild(_guild);
                var lobby = await _dataAccessLayer.dataRooms.GetLobby(guild_db.Id);

                if (lobby == null)
                    return;

                if (lobby.lobby_id == state2.VoiceChannel.Id)
                {
                    var _user = state2.VoiceChannel.GetUser(user.Id);
                    var _category = state2.VoiceChannel.CategoryId ?? 0;
                    await CreateRoom(_user, _guild, _category);
                }
            }

            // Выполеяется при выходи из приватного войса
            if (state1.VoiceChannel != null)
            {
                int _countVoiceUser = state1.VoiceChannel.ConnectedUsers.Count;
                ulong _id_channel = state1.VoiceChannel.Id;
                var _temp = await _dataAccessLayer.dataRooms.GetTempRoom(_id_channel);
                
                if (_countVoiceUser == 0 && _temp != null)
                {
                    await DestroyerRoom(state1.VoiceChannel);
                    return;
                }
                return;
            }
        }

        private async Task<RestVoiceChannel> CreateRoom(SocketGuildUser userOwner, SocketGuild guild, ulong category)
        {
           
            string name = $"{userOwner.Username}'s Lair";
            var room_settings = await _dataAccessLayer.dataRooms.CreateOrGetRoomSettings(userOwner.Id, name);

            var room = await guild.CreateVoiceChannelAsync($"{room_settings.Name}");
            await room.AddPermissionOverwriteAsync(guild.EveryoneRole, new OverwritePermissions(connect: PermValue.Inherit));
            await room.AddPermissionOverwriteAsync(userOwner, new OverwritePermissions(connect: PermValue.Allow, manageChannel: PermValue.Allow));

            await room.ModifyAsync(x => x.UserLimit = room_settings.limit);


            if (category != 0)
            {
                await room.ModifyAsync(x => x.CategoryId = category);
            }

            await userOwner.ModifyAsync(x =>
            {
                x.ChannelId = room.Id;
            });

            await _dataAccessLayer.dataRooms.CreateTempRoom(room.Id, userOwner.Id);

            Log.Debug("Create room OK");
            return room;
        }

        private async Task DestroyerRoom(SocketVoiceChannel channel)
        {
            await _dataAccessLayer.dataRooms.DeleteTempRoom(channel.Id);
            await channel.DeleteAsync();

        }
    } 
}