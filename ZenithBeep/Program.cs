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

        private BotOptions _botOption;

        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
       
        static void Main(string[] args) {

           
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program() {
            
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddYamlFile("appsettings.yml");

            _config = config.Build();
            _botOption = new BotOptions();
            _config.GetSection(_botOption.Bot).Bind(_botOption);
            
         
        }

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();


        public async Task MainAsync()
        {
          

           using (var services = ConfigureServices()) 
           {
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                var _sCommand = services.GetRequiredService<InteractionService>();
                await services.GetRequiredService<HandlerInteraction>().InitializeAsync();
                var audioService = services.GetRequiredService<IAudioService>();
                await services.GetRequiredService<HandlerStatus>().InitializeAsync();
                await services.GetRequiredService<HandlerRoles>().InitializeAsync();
                await services.GetRequiredService<HandlerJoinGuilds>().InitializeAsync();
                await services.GetRequiredService<HandlerJoinLobby>().InitializeAsync();
                services.GetRequiredService<LoggingService>();
                var context = services.GetRequiredService<BeepDbContext>();
                
                
                _client.Ready += async () =>
                {

                    switch (_botOption.DBSetup) {
                        case true:
                            await setupDatabaseTask(context);
                            break;
                        case false:
                            Log.Information("Skip setup database!");
                            break;
                    }


                    Console.WriteLine(" _____         _ _   _   _____            ");
                    Console.WriteLine("|__   |___ ___|_| |_| |_| __  |___ ___ ___");
                    Console.WriteLine("|   __| -_|   | |  _|   | __ -| -_| -_| . |");
                    Console.WriteLine("|_____|___|_|_|_|_| |_|_|_____|___|___|  _|");
                    Console.WriteLine("                                      |_|  ");

                    Console.WriteLine($"Ver: {System.Reflection.Assembly.GetEntryAssembly().GetName().Version} ");
                   
                    await _sCommand.RegisterCommandsGloballyAsync(true);

                    switch (_botOption.AudioService)
                    {
                        case true:
                            await audioService.InitializeAsync();
                            break;
                        case false:
                            break;
                    }

                    
                
                };

                Console.CancelKeyPress += OnCancel;

                await client.LoginAsync(TokenType.Bot, _botOption.Token);
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

        private static async Task setupDatabaseTask(BeepDbContext context) {
            Log.Information("====== Database setup starting ======");
            var migrations = await context.Database.GetPendingMigrationsAsync();
            if (migrations.Any()) {
                Log.Information("===== Migrations required: " + string.Join(", ", migrations) + " =====");
                await context.Database.MigrateAsync();
                await context.SaveChangesAsync();
            }
            
            await context.Database.EnsureCreatedAsync();
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

            var pathDb = DbSettings.LocalPathDB();

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
                .AddSingleton<HandlerInteraction>()
                .AddSingleton(x => new CommandService())
                .AddSingleton<LoggingService>()
                .AddSingleton<HandlerStatus>()
                .AddSingleton<IAudioService, LavalinkNode>()
                .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                .AddSingleton<PaginationService>()
                .AddSingleton<HandlerRoles>()
                .AddMicrosoftExtensionsLavalinkLogging()
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton(new LavalinkNodeOptions
                {
                    RestUri = $"http://{_botOption.LavaHost}:2333/",
                    WebSocketUri = $"ws://{_botOption.LavaHost}:2333/",
                    Password = _botOption.LavaPassword,


                })
                .AddSingleton<ILavalinkCache, LavalinkCache>()
                .AddSingleton<HandlerJoinLobby>()
                .AddSingleton<HandlerJoinGuilds>()
                .AddDbContextFactory<BeepDbContext>(
                    options => options.UseSqlite($"Data Source={pathDb}"))
                .AddSingleton<DataAccessLayer>()
                .AddSingleton<DataRooms>()
                .AddSingleton<ParseEmoji>();


            switch (_botOption.Logs.ToLower())
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
            
           
            var serviceProvider = services.BuildServiceProvider();
            
            return serviceProvider;
        }

    }
}
