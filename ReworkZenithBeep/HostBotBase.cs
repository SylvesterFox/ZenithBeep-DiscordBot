
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReworkZenithBeep.InteractionCommand.Utils;

namespace ReworkZenithBeep
{
    internal class HostBotBase : BackgroundService
    {
        public static IAudioService AudioService { get; private set; }

        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordClient _discordClient;

        public HostBotBase(IServiceProvider serviceProvider, DiscordClient discord)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(discord);

            this._serviceProvider = serviceProvider;
            this._discordClient = discord;
            AudioService = serviceProvider.GetRequiredService<IAudioService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var slash = _discordClient
                .UseSlashCommands(new SlashCommandsConfiguration
                {
                    Services = _serviceProvider
                });
            // Slash commands register
            slash.RegisterCommands<IntaractionUtilityCommand>();

            var next = _discordClient
                .UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefixes = ["$"],
                    Services = _serviceProvider
                });

            // Next command Register


            await _discordClient.ConnectAsync().ConfigureAwait(false);

            var readyTaskCompletionSource = new TaskCompletionSource();

            Task SetResult(DiscordClient client, ReadyEventArgs eventArgs)
            {
                readyTaskCompletionSource.TrySetResult();
                Console.WriteLine("Ready RAWR!");
                return Task.CompletedTask;
            }

            _discordClient.Ready += SetResult;
            await readyTaskCompletionSource.Task.ConfigureAwait(false);
            _discordClient.Ready -= SetResult;

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
        }
    }
}
