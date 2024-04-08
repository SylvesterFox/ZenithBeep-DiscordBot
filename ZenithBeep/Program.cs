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
using ZenithBeep.Player;
using ZenithBeep.Settings;




namespace ZenithBeep
{
    public class Program
    {
        // Без этого не работает Lavalink4NET
        private IConfigurationRoot _config => new ConfigurationBuilder().Build();
        private DiscordSocketClient _client;
        private BotConfig _botConfig;

        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
       
        static void Main(string[] args = null) {

            new Program().MainAsync().GetAwaiter().GetResult();
        }


        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public async Task MainAsync()
        {

            if (!Settings.SettingsManager.Instance.LoadConfiguration())
            {
                Console.WriteLine("Configuration is not loaded\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

           _botConfig = Settings.SettingsManager.Instance.LoadedConfig;

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

                try
                {
                    var player = services.GetRequiredService<IAudioService>();
                    await player.StartAsync();
                }
                catch (Exception ex)
                {
                    Log.Warning("Lavalink service not starting!");
                }



                services.GetRequiredService<LoggingService>();
                var context = services.GetRequiredService<BeepDbContext>();
                

                _client.Ready += async () =>
                {
                    Console.WriteLine("RAWR! Bot is ready!");
                    await _sCommand.RegisterCommandsGloballyAsync(true);
                   
                };

                Console.CancelKeyPress += OnCancel;

                await client.LoginAsync(TokenType.Bot, _botConfig.TOKEN);
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

            var services = new ServiceCollection();

            AddDiscordServices(services);
            AddDatabaseServices(services);
            AddLogging(services);
            if (_botConfig.AUDIOSERICES)
                AddLavalink(services);

            return services.BuildServiceProvider();
        }

        private void AddDiscordServices(IServiceCollection services)
        {
            services.AddSingleton(_config)
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
                .AddSingleton<HandlerJoinLobby>()
                .AddSingleton<HanderJoinGuilds>()
                .AddSingleton<DataAccessLayer>()
                .AddSingleton<DataRooms>()
                .AddSingleton<ParseEmoji>()
                .AddSingleton<GetExtension>();
        }

        private void AddDatabaseServices(IServiceCollection services)
        {
            services.AddDbContextFactory<BeepDbContext>(options =>
            options.UseNpgsql($"Host={_botConfig.POSTGRES_HOST};Database=${_botConfig.POSTGRES_DB};Username={_botConfig.POSTGRES_USER};Password={_botConfig.POSTGRES_PASSWORD}"));
        }

        private void AddLavalink(IServiceCollection services)
        {
            services.AddSingleton<MusicZenithHelper>()
                .AddLavalink()
                .ConfigureLavalink(config =>
                {
                    config.BaseAddress = new Uri(_botConfig.LAVALINK_ADDRES);
                    config.WebSocketUri = new Uri(_botConfig.LAVALINK_WEBSOCKET);
                    config.ReadyTimeout = TimeSpan.FromSeconds(10);
                    config.Passphrase = _botConfig.LAVALINK_PASSWORD;
                });

        }

        private void AddLogging(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog());

            if (string.IsNullOrEmpty(_botConfig.LOGS))
            {
                ConfigureLogger(LogEventLevel.Information);
            }
            else
            {
                LogEventLevel logLevel;
                if (!Enum.TryParse(_botConfig.LOGS, true, out logLevel))
                {
                    logLevel = LogEventLevel.Information;
                }
                ConfigureLogger(logLevel);
            }

        }
        
    }
}
