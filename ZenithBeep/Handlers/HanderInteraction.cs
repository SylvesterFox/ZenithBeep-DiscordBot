using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ZenithBeep.Custom;
using ZenithBeep.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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


        public HanderInteraction(DiscordSocketClient client, InteractionService command, IServiceProvider service)
        {
            _client = client;
            _command = command;
            _service = service;
            _log = service.GetRequiredService<ILogger<LoggingService>>();

            if (Settings.SettingsManager.Instance.LoadedConfig.AUDIOSERICES)
                _musicEvents = service.GetRequiredService<MusicEvents>();
        }

        public async Task InitializeAsync()
        {
            await ModuleInitializeAsync();

            _client.InteractionCreated += HandlerInteraction;

            // Buttons
            if (Settings.SettingsManager.Instance.LoadedConfig.AUDIOSERICES)
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

                        var embedError = CustomEmbeds.ErrorEmbed($"Error: {ex.GetType()} | {ex.Message}");

                        await arg.RespondOrFollowup(embed: embedError, ephemeral: true);
                        Log.Error($"Error: {ex.GetType()} | {ex.Message}");
                    }
                    break;
                case InteractionType.ApplicationCommandAutocomplete:
                    break;
                case InteractionType.MessageComponent:
                    break;
                default:
                    Log.Warning("Unsupported interaction type: " + arg.Type);
                    break;
            }
        }

        private async Task HandleModal(ModalCommandInfo info, IInteractionContext context, IResult result)
        {
            Embed embed;
            if (!result.IsSuccess && result is ZenithResult mResult)
            {

                embed = CustomEmbeds.ErrorEmbed($"Error {mResult.ErrorReason} | {mResult.Message}");

                if (mResult.Error == InteractionCommandError.ParseFailed)
                {
                    embed = CustomEmbeds.WarningEmbed($"{mResult.Message}", $"{mResult.ErrorReason}");
                }

                await context.Interaction.RespondOrFollowup(embed: embed, ephemeral: true);
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
  
            Embed embed;

            if (result is ZenithResult mResult)
            {
                embed = CustomEmbeds.ErrorEmbed($"Error {mResult.ErrorReason} | {mResult.Message}");

                if (mResult.Error == InteractionCommandError.ParseFailed)
                {
                    embed = CustomEmbeds.WarningEmbed($"{mResult.Message}", $"{mResult.ErrorReason}");
                }

                await context.Interaction.RespondOrFollowup(embed: embed, ephemeral: true);
                return;
            }

            embed = CustomEmbeds.ErrorEmbed("Error: " + (result.Error?.ToString() ?? "Oof") + "\n" + result.ErrorReason ?? "Something broke");
                

            await context.Interaction.RespondOrFollowup(embed: embed, ephemeral: true);
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


    }
}
