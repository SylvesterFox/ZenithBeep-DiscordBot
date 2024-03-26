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
<<<<<<< HEAD
=======
                await player.StartAsync();

>>>>>>> parent of b9a63d5 (Dev-commit #2)
                services.GetRequiredService<LoggingService>();
                var context = services.GetRequiredService<BeepDbContext>();
                
                

                _client.Ready += async () =>
                {
                    if (_config["ARCHIVAL_MODE"] == "true")
                    {
                        Log.Information("!!! ARCHIVE MODE ONLY !!!");
                    } else
                    {
                        await setupDatabase(context);
                    }
                    Console.WriteLine("RAWR! Bot is ready!");
                    await _sCommand.RegisterCommandsGloballyAsync(true);
                    await player.StartAsync();
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
                .AddDbContextFactory<BeepDbContext>( options => options.UseNpgsql($"Host={_config["POSTGRES_HOST"]};Database={_config["POSTGRES_DB"]};Username={_config["POSTGRES_USER"]};Password={_config["POSTGRES_PASSWORD"]};Port={_config["POSTGRES_PORT"]}"))
                .AddSingleton<DataAccessLayer>()
                .AddSingleton<DataRooms>()
                .AddSingleton<ParseEmoji>()
                .AddLavalink()
                .ConfigureLavalink(config => {
                    config.BaseAddress = new Uri(_config["LAVALINK_ADDRESS"]);
                    config.WebSocketUri = new Uri(_config["LAVALINK_WEBSOCKET"]);
                    config.ReadyTimeout = TimeSpan.FromSeconds(20);
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

        private static async Task setupDatabase(BeepDbContext ctx)
        {
            Log.Information("Setting up database");
            var migrations = await ctx.Database.GetPendingMigrationsAsync();
            if (migrations.Any())
            {
                Log.Information("Migrations required: " + string.Join(", ", migrations) + ".");
                await ctx.Database.MigrateAsync();
                await ctx.SaveChangesAsync();
            }

            await ctx.Database.EnsureCreatedAsync();

        }

        
    }
}
