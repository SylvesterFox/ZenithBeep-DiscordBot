using DSharpPlus;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReworkZenithBeep.Module.Music;
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
            builder.Services.ConfigureLavalink(options =>
            {
                options.Passphrase = _botConfig.LAVALINK_PASSWORD;
                options.BaseAddress = new Uri(_botConfig.LAVALINK_ADDRES);
                options.WebSocketUri = new Uri(_botConfig.LAVALINK_WEBSOCKET);
                options.ReadyTimeout = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddSingleton<MusicCommand>();
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
