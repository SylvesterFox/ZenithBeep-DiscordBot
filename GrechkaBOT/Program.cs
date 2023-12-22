using Discord;
using Serilog;
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


namespace Csharp_GrechkaBot
{
    public class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;

        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
       
        static void Main(string[] args = null) {

           
            new Program().MainAsync().GetAwaiter().GetResult();
        }

         public Program() {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddYamlFile("appsettings.yml");

            _config = config.Build();
        }

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();


        public async Task MainAsync()
        {
           
           using (var services = ConfigureServices()) 
           {
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                var _sCommand = services.GetRequiredService<InteractionService>();
                await services.GetRequiredService<HanderInteraction>().InitializeAsync();
                var audioService = services.GetRequiredService<IAudioService>();
                await services.GetRequiredService<HandlerStatus>().InitializeAsync();
                await services.GetRequiredService<HanderRoles>().InitializeAsync();
                await services.GetRequiredService<HanderJoinGuilds>().InitializeAsync();
                await services.GetRequiredService<HandlerJoinLobby>().InitializeAsync();
                services.GetRequiredService<LoggingService>();
                services.GetRequiredService<ConnectionDB>();

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

                    await audioService.InitializeAsync();
                };

                Console.CancelKeyPress += OnCancel;

                await client.LoginAsync(TokenType.Bot, _config["token"]);
                await client.StartAsync();

                try {
                    await Task.Delay(-1, cancellation.Token);
                }
                catch (TaskCanceledException) {
                    await client.StopAsync();
                    await client.LogoutAsync();
                }

                
           }

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


        private ServiceProvider ConfigureServices() 
        {
            var services = new ServiceCollection()
                .AddSingleton(_config)
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
                .AddSingleton<LoggingService>()
                .AddSingleton<HandlerStatus>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                .AddSingleton<PaginationService>()
                .AddSingleton<HanderRoles>()
                .AddMicrosoftExtensionsLavalinkLogging()
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton(new LavalinkNodeOptions {
                    RestUri = "http://localhost:2333/",
                    WebSocketUri = "ws://localhost:2333/",
                    Password = "youshallnotpass"

                })
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .AddSingleton<ConnectionDB>()
                .AddSingleton<HandlerJoinLobby>()
                .AddSingleton<HanderJoinGuilds>();

            if (!string.IsNullOrEmpty(_config["logs"]))
            {


                switch (_config["logs"].ToLower())
                {
                    case "info": 
                    {
                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                            .MinimumLevel.Information()
                            .CreateLogger();
                        break;
                    }
                    case "error": 
                    {


                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                            .MinimumLevel.Error()
                            .CreateLogger();
                        break;
                    }
                    case "debug":
                    {
                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                            .MinimumLevel.Debug()
                            .CreateLogger();
                        break;
                    }
                    default: 
                    {
                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                            .MinimumLevel.Information()
                            .CreateLogger();
                        break;
                    }
                }
            }
            else {
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                        .MinimumLevel.Information()
                        .CreateLogger();
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

    }
}
