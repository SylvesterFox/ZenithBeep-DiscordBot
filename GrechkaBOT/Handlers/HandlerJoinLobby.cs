
using Discord.WebSocket;
using GrechkaBOT.Database;

namespace GrechkaBOT.Handlers
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

        private async Task<bool> GetIDLobby(long ChannelId) 
        {
            var _dblobbyId = new ModelRoomsLobby { lobby_id = ChannelId };
            ModelRoomsLobby _get = DatabasePost.GetIdChannelLobby<ModelRoomsLobby>(_dblobbyId);
            if (_get != null) {
                return true;
            }

            return false;
        }

        private async Task OnCreatePrivateRoom(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            if (user.IsBot)
                return;

            if (state2.VoiceChannel != null) {
                bool _checkLobby = await GetIDLobby((long)state2.VoiceChannel.Id);
                Console.WriteLine(state2.VoiceChannel.ConnectedUsers.Count);
                if (_checkLobby)
                    await CreateRoom(user);
                    
            }

            if (state1.VoiceChannel != null) {
                int _countVoiceUser = state1.VoiceChannel.ConnectedUsers.Count;
                if (_countVoiceUser == 0) {
                    await DestroyerRoom();
                    return;
                }
                return;
            }
        }

        private async Task CreateRoom(SocketUser UserOwner) 
        {
            Console.WriteLine("Create room OK");
        }

        private async Task DestroyerRoom() {
            Console.WriteLine("null");
        }
    }
}