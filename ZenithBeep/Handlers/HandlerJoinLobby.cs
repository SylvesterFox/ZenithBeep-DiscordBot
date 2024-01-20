using Discord;
using Discord.Rest;
using Discord.WebSocket;


namespace ZenithBeep.Handlers
{
    public class HandlerJoinLobby
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _service;


        public HandlerJoinLobby(DiscordSocketClient client, IServiceProvider serviceProvider) 
        {
            _client = client;
            _service = serviceProvider;
        }


        public async Task InitializeAsync() 
        {
            _client.UserVoiceStateUpdated += OnCreatePrivateRoom;
        }

/*
        private async Task<ModelGuild> GetGuildKey(ulong guildId) {
             var _get_guild_key = new ModelGuild {
                guildId = (long)guildId
            };
            ModelGuild _key = DatabasePost.GetGuild<ModelGuild>(_get_guild_key);
            return _key;
        }


        private async Task<ModelRoomsLobby> GetIDLobby(int guildKey) {
            var _dblobbyId = new ModelRoomsLobby { guild_key = guildKey };
            ModelRoomsLobby _get = DatabasePost.GetIdChannelLobby<ModelRoomsLobby>(_dblobbyId);
            return _get;
        }
*/
        private async Task OnCreatePrivateRoom(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            if (user.IsBot)
                return;
/*
            // Выполняется при заходи в лоби по созданию приватного войса
            if (state2.VoiceChannel != null) {
                SocketGuild _guild = state2.VoiceChannel.Guild;
                ModelGuild key_guild = await GetGuildKey(_guild.Id);
                var _lobby = await GetIDLobby(key_guild.Id);

                if ((ulong)_lobby.lobby_id == state2.VoiceChannel.Id) {
                    var _user = state2.VoiceChannel.GetUser(user.Id);
                    var _category = state2.VoiceChannel.CategoryId ?? 0;
                    await CreateRoom(_user, _guild, _category);                    
                }
            }

            // Выполеяется при выходи из приватного войса
            if (state1.VoiceChannel != null) {
                int _countVoiceUser = state1.VoiceChannel.ConnectedUsers.Count;
                ulong _id_channel = state1.VoiceChannel.Id;
                var _temp = new ModelTempRoom {
                    channel_room = (long)_id_channel
                };
                ModelTempRoom temproom = DatabasePost.GetTempRoom<ModelTempRoom>(_temp);
                if (_countVoiceUser == 0 && temproom != null) {
                    await DestroyerRoom(state1.VoiceChannel);
                    return;
                }
                return;
            }*/
        }

       /* private async Task<RestVoiceChannel> CreateRoom(SocketGuildUser userOwner, SocketGuild guild, ulong category) 
        { 
            var _req = new ModelRooms {
                channel_owmer = (long)userOwner.Id
            };
            ModelRooms _get = DatabasePost.GetRoom<ModelRooms>(_req);
            string name = $"Voice-{userOwner.Username}";
            int limit_vc = 0;

            if (_get != null)
            {
                name = _get.name;
                limit_vc = _get.limit;
            } else {

                var _addSettings = new ModelRooms{
                    channel_owmer = (long)userOwner.Id,
                    name = name
                };     
                DatabasePost.insertSettingRoom(_addSettings);
            }
            
            var room = await guild.CreateVoiceChannelAsync($"{name}");
            await room.AddPermissionOverwriteAsync(guild.EveryoneRole, new OverwritePermissions(connect: PermValue.Inherit));
            await room.AddPermissionOverwriteAsync(userOwner, new OverwritePermissions(connect: PermValue.Allow));

            if (limit_vc != 0) {
                await room.ModifyAsync(x => x.UserLimit = limit_vc);
            }

            var _temp = new ModelTempRoom {
                channel_room = (long)room.Id,
                id_user = (long)userOwner.Id
            };

            if (category != 0) {
                await room.ModifyAsync(x => x.CategoryId = category);
            }

            await userOwner.ModifyAsync(x => {
                x.ChannelId = room.Id;
            });

            DatabasePost.insertTempRoom(_temp);
            Console.WriteLine("Create room OK");
            return room;
        }

        private async Task DestroyerRoom(SocketVoiceChannel channel) {;
            DatabasePost.deleteTempRoom((long)channel.Id);
            await channel.DeleteAsync();
      
        } */
    } 
}