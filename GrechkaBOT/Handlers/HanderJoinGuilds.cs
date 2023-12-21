using Discord.WebSocket;
using GrechkaBOT.Database;

namespace GrechkaBOT.Handlers
{
    public class HanderJoinGuilds
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;


        public HanderJoinGuilds(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            _client.JoinedGuild += OnBotJoinGuild;
        }

        private Task OnBotJoinGuild(SocketGuild guild)
        {
            var guildInfo = new ModelGuild
            {
                Name = guild.Name,
                guildId = (long)guild.Id,
                Leng = "us"
            };

            DatabasePost.insertGuild(guildInfo);
            return Task.CompletedTask;
        }
    }
}
