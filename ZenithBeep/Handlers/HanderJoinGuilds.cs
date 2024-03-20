using Discord.WebSocket;
using ZenithBeepData;


namespace ZenithBeep.Handlers
{
    public class HanderJoinGuilds
    {
        private readonly DiscordSocketClient _client;
        private readonly DataAccessLayer _db;


        public HanderJoinGuilds(DiscordSocketClient client, IServiceProvider serviceProvider, DataAccessLayer dataAccessLayer)
        {
            _client = client;
            _db = dataAccessLayer;
        }

        public async Task InitializeAsync()
        {
            _client.JoinedGuild += OnBotJoinGuild;
        }

        private async Task OnBotJoinGuild(SocketGuild guild)
        {
            await _db.CreateGuild(guild);
        }
    }
}
