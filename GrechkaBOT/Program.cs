using Discord;
using Serilog;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Custom;
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
using Microsoft.Extensions.Options;

namespace Csharp_GrechkaBot
{
    public class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private static string _logLevel;

       
        static void Main(string[] args = null) {

            _logLevel = "debug";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/csgrechka-logs.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
           
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

                services.GetRequiredService<LoggingService>();

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

                        /*Console.WriteLine(info.Id);*/

                        if (info == null)
                        {
                            listGuild.Add(guild_db);
                        }
                        
                    }

                    DatabasePost.insertGuild(listGuild);
                };

                await client.LoginAsync(TokenType.Bot, _config["token"]);
                await client.StartAsync();

                await Task.Delay(-1);
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
                .AddSingleton<HanderJoinGuilds>();

            if (!string.IsNullOrEmpty(_logLevel))
            {
                switch (_logLevel.ToLower())
                {
                    case "info": 
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                        break;
                    }
                    case "error": 
                    {

                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    }
                    case "debug":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                        break;
                    }
                    default: 
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    }
                }
            }
            else {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

    //     public async Task RunAsync(IHost host)
    //     {
    //         using IServiceScope servicescope = host.Services.CreateScope();
    //         IServiceProvider provider = servicescope.ServiceProvider;

    //         var _client = provider.GetRequiredService<DiscordSocketClient>();
    //         var _sCommand = provider.GetRequiredService<InteractionService>();
    //         await provider.GetRequiredService<HanderInteraction>().InitializeAsync();
    //         var _config = provider.GetRequiredService<IConfigurationRoot>();

            

    //         var log = provider.GetRequiredService<HandlerLog>();
    //         var status = provider.GetService<HandlerStatus>()
    //             .InitializeAsync();
    //         var roles = provider.GetService<HanderRoles>().InitializeAsync();
    //         var JoinGuild = provider.GetService<HanderJoinGuilds>().InitializeAsync();

    //         var audioService = provider.GetRequiredService<IAudioService>();
            

    //         _client.Ready += async () =>
    //         {
    //             Console.WriteLine("RAWR! Bot is ready!");
    //             await _sCommand.RegisterCommandsGloballyAsync();

    //             var listGuild = new List<ModelGuild>();
    //             foreach (var guild in _client.Guilds)
    //             {

    //                 var guild_db = new ModelGuild
    //                 {
    //                     Name = guild.Name,
    //                     guildId = (long)guild.Id,
    //                     Leng = "us"
    //                 };

    //                 var get = new ModelGuild { guildId = (long)guild.Id };

    //                 ModelGuild info = DatabasePost.GetGuild<ModelGuild>(get);

    //                 /*Console.WriteLine(info.Id);*/

    //                 if (info == null)
    //                 {
    //                     listGuild.Add(guild_db);
    //                 }
                    
    //             }

    //             DatabasePost.insertGuild(listGuild);
    //         };

    //         await _client.LoginAsync(Discord.TokenType.Bot, _config["token"]);
    //         await _client.StartAsync();

    //         await audioService.InitializeAsync();

    //         await Task.Delay(-1, tokenSource.Token);
    //     }
    }
}
