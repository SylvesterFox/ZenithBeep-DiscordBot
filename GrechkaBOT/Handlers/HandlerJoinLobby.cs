
using Discord.WebSocket;

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

        private async Task OnCreatePrivateRoom(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            if (user.IsBot)
                return;

            if (state2.VoiceChannel != null) {
                Console.WriteLine($"User {user.Username}");
                // await CreateRoom();
            }
        }

        // private async Task CreateRoom() 
        // {

        // }
    }
}