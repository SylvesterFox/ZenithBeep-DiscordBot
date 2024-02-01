using Discord;
using Serilog;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using ZenithBeep.Handlers;
using ZenithBeep.Services;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Logging.Microsoft;
using Lavalink4NET.MemoryCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ZenithBeepData.Context;
using ZenithBeepData;
using ZenithBeep.Custom;


namespace ZenithBeep
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
                
                

                _client.Ready += async () =>
                {
                    Console.WriteLine("RAWR! Bot is ready!");

                   
                    await _sCommand.RegisterCommandsGloballyAsync(true);


                    string audio = _config["audioservice"];
                    switch (audio)
                    {
                        case "true":
                            await audioService.InitializeAsync();
                            break;
                        case "false":
                            break;
                        default:
                            await audioService.InitializeAsync();
                            break;
                    }
                    
                   
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
                    UseInteractionSnowflakeDate = false
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
                .AddSingleton(new LavalinkNodeOptions
                {
                    RestUri = $"http://{_config["lavalink_host"]}:2333/",
                    WebSocketUri = $"ws://{_config["lavalink_host"]}:2333/",
                    Password = _config["lavalink_password"],


                })
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .AddSingleton<HandlerJoinLobby>()
                .AddSingleton<HanderJoinGuilds>()
                .AddDbContextFactory<BeepDbContext>(
                    options => options.UseNpgsql(_config.GetConnectionString("Default")))
                .AddSingleton<DataAccessLayer>()
                .AddSingleton<DataRooms>()
                .AddSingleton<ParseEmoji>();




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
