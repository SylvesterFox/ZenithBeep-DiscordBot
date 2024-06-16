using DSharpPlus;
using Lavalink4NET.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Services;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep
{
    internal class Program
    {
        public static BotConfig _botConfig;
        static void Main(string[] args)
        {


            if (!Settings.SettingsManager.Instance.LoadConfiguration())
            {
                Console.WriteLine("Configuration is not loaded\nPress any key to exit...");
                return;
            }

            _botConfig = Settings.SettingsManager.Instance.LoadedConfig;

            var builder = new HostApplicationBuilder();
            string tokendb = "server=localhost;database=Zenith;User=root;Password=8342003";
            builder.Services.AddDbContextFactory<BotContext>(o => o.UseMySql(tokendb, ServerVersion.AutoDetect(tokendb), x =>
                    x.MigrationsAssembly("ReworkZenithBeep.Data.Migrations")));
            builder.Services.AddHostedService<HostBotBase>();
            builder.Services.AddSingleton<DiscordClient>();
            builder.Services.AddSingleton(new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = _botConfig.TOKEN,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
            builder.Services.AddLavalink();
            builder.Services.AddSingleton<DataBot>();
            builder.Services.ConfigureLavalink(options =>
            {
                options.Passphrase = _botConfig.LAVALINK_PASSWORD;
                options.BaseAddress = new Uri(_botConfig.LAVALINK_ADDRES);
                options.WebSocketUri = new Uri(_botConfig.LAVALINK_WEBSOCKET);
                options.ReadyTimeout = TimeSpan.FromSeconds(10);
            });
            
            builder.Services.AddSingleton<PaginationService>();
            

            builder.Services.AddLogging(s => s.AddConsole()
            #if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
            #else            
            .SetMinmumLevel(LogLevel.Information)
            #endif
            );
            

            var host = builder.Build();
            AppDomain.CurrentDomain.ProcessExit += (object? _, EventArgs _) => { host.Dispose(); };
            host.Run();
        }
    }
}
