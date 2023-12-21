using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Database;
using GrechkaBOT.Handlers;
using GrechkaBOT.Services;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Logging.Microsoft;
using Lavalink4NET.MemoryCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Csharp_GrechkaBot
{
    public class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public async Task MainAsync()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddYamlFile("appsettings.yml").Build();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                services
                .AddSingleton(config)
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = Discord.GatewayIntents.All,
                    AlwaysDownloadUsers = true,
                    LogLevel = Discord.LogSeverity.Debug,
                    HandlerTimeout = 5000,
                    MessageCacheSize = 1000,
                    DefaultRetryMode = Discord.RetryMode.RetryRatelimit,
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<HanderInteraction>()
                .AddSingleton(x => new CommandService())
                .AddSingleton<HandlerLog>()
                .AddSingleton<HandlerStatus>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                .AddSingleton<PaginationService>()
                .AddSingleton<HanderRoles>()
                .AddMicrosoftExtensionsLavalinkLogging()
                .AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.None))
                .AddSingleton(new LavalinkNodeOptions {
                    RestUri = "http://localhost:2333/",
                    WebSocketUri = "ws://localhost:2333/",
                    Password = "youshallnotpass"

                })
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .AddSingleton<ConnectionDB>()
                .AddSingleton<HanderJoinGuilds>()
                .AddSingleton<HandlerJoinLobby>()
                ).Build();

            await RunAsync(host);
        }

        private void OnCancel(object? sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;

            using (tokenSource)
            {
                tokenSource.Cancel();
                Thread.Sleep(1000);
            }
        }

        public async Task RunAsync(IHost host)
        {
            using IServiceScope servicescope = host.Services.CreateScope();
            IServiceProvider provider = servicescope.ServiceProvider;

            var _client = provider.GetRequiredService<DiscordSocketClient>();
            var _sCommand = provider.GetRequiredService<InteractionService>();
            await provider.GetRequiredService<HanderInteraction>().InitializeAsync();
            var _config = provider.GetRequiredService<IConfigurationRoot>();
            var log = provider.GetRequiredService<HandlerLog>();
            var status = provider.GetService<HandlerStatus>()
                .InitializeAsync();
            var roles = provider.GetService<HanderRoles>().InitializeAsync();
            var JoinGuild = provider.GetService<HanderJoinGuilds>().InitializeAsync();
            var JoinLobby = provider.GetService<HandlerJoinLobby>().InitializeAsync();

            var audioService = provider.GetRequiredService<IAudioService>();
            

            _client.Ready += async () =>
            {
                Console.WriteLine("RAWR! Bot is ready!");
                await _sCommand.RegisterCommandsGloballyAsync();

                var listGuild = new List<ModelGuild>();
                foreach (var guild in _client.Guilds)
                {

                    var guild_db = new ModelGuild
                    {
                        Name = guild.Name,
                        guildId = (long)guild.Id,
                        Leng = "us"
                    };

                    var get = new ModelGuild { guildId = (long)guild.Id };

                    ModelGuild info = DatabasePost.GetGuild<ModelGuild>(get);


                    if (info == null)
                    {
                        listGuild.Add(guild_db);
                    }
                    
                }

                DatabasePost.insertGuild(listGuild);
            };

            await _client.LoginAsync(Discord.TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await audioService.InitializeAsync();

            await Task.Delay(-1, tokenSource.Token);
        }
    }
}
