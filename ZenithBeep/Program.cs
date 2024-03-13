using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ZenithBeep.Handlers;
using ZenithBeep.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenithBeepData.Context;
using Discord.Commands;
using Serilog;
using Microsoft.EntityFrameworkCore;
using ZenithBeepData;
using ZenithBeep.Custom;
using Serilog.Events;
using Lavalink4NET.Extensions;
using Lavalink4NET;



namespace ZenithBeep
{
    public class Program
    {
        private IConfigurationRoot _config;
        private DiscordSocketClient _client;

        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
       
        static void Main(string[] args = null) {

            new Program().MainAsync().GetAwaiter().GetResult();
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
                await services.GetRequiredService<HandlerStatus>().InitializeAsync();
                await services.GetRequiredService<HanderRoles>().InitializeAsync();
                await services.GetRequiredService<HanderJoinGuilds>().InitializeAsync();
                await services.GetRequiredService<HandlerJoinLobby>().InitializeAsync();
                var player = services.GetRequiredService<IAudioService>();
                await player.StartAsync();

                services.GetRequiredService<LoggingService>();
                var context = services.GetRequiredService<BeepDbContext>();
                
                

                _client.Ready += async () =>
                {
                    Console.WriteLine("RAWR! Bot is ready!");
                    await _sCommand.RegisterCommandsGloballyAsync(true);
                   
                };

                Console.CancelKeyPress += OnCancel;

                await client.LoginAsync(TokenType.Bot, _config["TOKEN"]);
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

        void ConfigureLogger(LogEventLevel eventLevel) {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Is(eventLevel)
                .CreateLogger();
        }

        public ServiceProvider ConfigureServices() 
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            CommonConfigService.Load(dotenv);

            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            
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
                .AddSingleton<PaginationService>()
                .AddSingleton<HanderRoles>()
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton<HandlerJoinLobby>()
                .AddSingleton<HanderJoinGuilds>()
                .AddDbContextFactory<BeepDbContext>( options => options.UseNpgsql(_config.GetConnectionString("db")))
                .AddSingleton<DataAccessLayer>()
                .AddSingleton<DataRooms>()
                .AddSingleton<ParseEmoji>()
                .AddLavalink()
                .ConfigureLavalink(config => {
                    config.BaseAddress = new Uri(_config["LAVALINK_ADDRESS"]);
                    config.WebSocketUri = new Uri(_config["LAVALINK_WEBSOCKET"]);
                    config.ReadyTimeout = TimeSpan.FromSeconds(10);
                    config.Passphrase = _config["LAVALINK_PASSWORD"];
                })
                .AddSingleton<BeepDbContext>();

            if(string.IsNullOrEmpty(_config["LOGS"]))
            {
                ConfigureLogger(LogEventLevel.Information);
            }
            else {
                LogEventLevel logLevel;
                if (!Enum.TryParse(_config["LOGS"], true, out logLevel)) {
                    logLevel = LogEventLevel.Information;
                }
                ConfigureLogger(logLevel);
            }

            var serviceProvider = services.BuildServiceProvider();
            
            return serviceProvider;
        }

        
    }
}
