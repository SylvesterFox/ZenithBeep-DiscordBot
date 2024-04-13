using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ZenithBeep.Custom;
using ZenithBeep.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ZenithBeep.Modeles;
using ZenithBeep.Player;
using Serilog;

namespace ZenithBeep.Handlers
{
    public class HanderInteraction
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _command;
        private readonly IServiceProvider _service;
        private readonly MusicEvents _musicEvents;


        private readonly Microsoft.Extensions.Logging.ILogger _log;
  

        public HanderInteraction(DiscordSocketClient client, InteractionService command, IServiceProvider service, MusicEvents musicEvents)
        {
            _client = client;
            _command = command;
            _service = service;
            _log = service.GetRequiredService<ILogger<LoggingService>>();
            _musicEvents = musicEvents;
        }

        public async Task InitializeAsync()
        {
            await ModuleInitializeAsync();

            _client.InteractionCreated += HandlerInteraction;
            _client.ButtonExecuted += _musicEvents.ButtonSearchClicked;

            _command.ContextCommandExecuted += ContextCommandExecuted;
            _command.SlashCommandExecuted += SlashCommandExecuted;
            _command.ModalCommandExecuted += HandleModal;
            _command.ComponentCommandExecuted += ComponentCommandExecuted;
            _command.Log += OnLogAsync;
            

        }

        private async Task ModuleInitializeAsync()
        {
            // Others commands
            await _command.AddModuleAsync<IntaractionModuleOthers>(_service);
            // Utility commands
            await _command.AddModuleAsync<IntaractionModuleUtility>(_service);
            // Roles commands
            await _command.AddModuleAsync<IntaractionModuleRoles>(_service);
            // Music commands
            if (Settings.SettingsManager.Instance.LoadedConfig.AUDIOSERICES)
                await _command.AddModuleAsync<IntaractionModuleMusic>(_service);

            // Romers commands
            await _command.AddModuleAsync<IntaractionModuleRoomers>(_service);


        }

        private async Task HandlerInteraction(SocketInteraction arg)
        {
           switch (arg.Type)
            {
                case InteractionType.ApplicationCommand:
                    try
                    {
                        var ctx = new SocketInteractionContext(_client, arg);
                        await _command.ExecuteCommandAsync(ctx, _service);
                    }
                    catch (Exception ex)
                    {
                        Embed embed = new EmbedBuilder()
                        {
                            Title = "Error: " + ex.GetType(),
                            Color = Color.Red,
                            Description = ex.Message
                        }.Build();

                        await arg.RespondOrFollowup(embed: embed, ephemeral: true);
                    }
                    break;
                case InteractionType.ApplicationCommandAutocomplete:
                    break;
                case InteractionType.MessageComponent:
                    /*await MusicEvents.ButtonSearchClicked(arg); */break;
                default:
                    Log.Warning("Unsupported interaction type: " + arg.Type);
                    break;
            }
        }

        private async Task HandleModal(ModalCommandInfo info, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess && result is ZenithResult mResult)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Error: " + mResult.ErrorReason,
                    Color = Color.Red,
                    Description = mResult.Message
                };

                if (mResult.Error == InteractionCommandError.ParseFailed)
                {
                    embed.Color = Color.Orange;
                }

                await context.Interaction.RespondOrFollowup(embed: embed.Build(), ephemeral: true);
            }
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private async Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                return;
            }
  
            EmbedBuilder embed;

            if (result is ZenithResult mResult)
            {
                embed = new EmbedBuilder()
                {
                    Title = "Error: " + mResult.ErrorReason,
                    Color = Color.Red,
                    Description = mResult.Message
                };

                if (mResult.Error == InteractionCommandError.ParseFailed)
                {
                    embed.Color = Color.Orange;
                }

                await context.Interaction.RespondOrFollowup(embed: embed.Build(), ephemeral: true);
                return;
            }

            embed = new EmbedBuilder()
            {
                Title = "Error: " + (result.Error?.ToString() ?? "Oof :/"),
                Color = Color.Red,
                Description = result.ErrorReason ?? "Something broke."
            };

            await context.Interaction.RespondOrFollowup(embed: embed.Build(), ephemeral: true);
        }

        private Task OnLogAsync(LogMessage msg)
        {
            string txt = $"{DateTime.Now,-8:hh:mm:ss} {$"[{msg.Severity}]",-9} {msg.Source,-8} | {msg.Exception?.ToString() ?? msg.Message}";
             switch (msg.Severity.ToString())
                {
                    case "Critical":
                    {
                        _log.LogCritical(txt);
                        break;
                    }
                    case "Warning":
                    {
                        _log.LogWarning(txt);
                        break;
                    }
                    case "Info":
                    {
                        _log.LogInformation(txt);
                        break;
                    }
                    case "Verbose":
                    {
                        _log.LogInformation(txt);
                        break;
                    } 
                    case "Debug":
                    {
                        _log.LogDebug(txt);
                        break;
                    } 
                    case "Error":
                    {
                        _log.LogError(txt);
                        break;
                    } 
                }

            return Task.CompletedTask;
        }

        // private static void OnLogAsync(object? obj, LogMessageEventArgs args)
        // {
        //     string txt = $"{DateTime.Now,-8:hh:mm:ss} {$"[{args.Level}]",-9} {args.Source,-8} | {args.Exception?.ToString() ?? args.Message}";
        //     Console.WriteLine(txt);
        // }

    }
}
