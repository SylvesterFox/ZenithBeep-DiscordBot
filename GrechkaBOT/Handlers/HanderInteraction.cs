
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrechkaBOT.Custom;
using Lavalink4NET.Logging;
using System.Reflection;

namespace GrechkaBOT.Handlers
{
    public class HanderInteraction
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _command;
        private readonly IServiceProvider _service;
        private readonly ILogger _logger;
  

        public HanderInteraction(DiscordSocketClient client, InteractionService command, IServiceProvider service, ILogger log)
        {
            _client = client;
            _command = command;
            _service = service;
            _logger = log;
        }

        public async Task InitializeAsync()
        {
            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

            _client.InteractionCreated += HandlerInteraction;

            _command.ContextCommandExecuted += ContextCommandExecuted;
            _command.SlashCommandExecuted += SlashCommandExecuted;
            _command.ModalCommandExecuted += HandleModal;
            _command.ComponentCommandExecuted += ComponentCommandExecuted;
            _command.Log += OnLogAsync;

            if (_logger is EventLogger log)
            {
                log.LogMessage += OnLogAsync;
            }
        }

    

        private async Task HandlerInteraction(SocketInteraction arg)
        {
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
            /*    if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());*/
            }
        }

        private async Task HandleModal(ModalCommandInfo info, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess && result is GrechkaResult mResult)
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

            if (result is GrechkaResult mResult)
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

        private static Task OnLogAsync(LogMessage msg)
        {
            string txt = $"{DateTime.Now,-8:hh:mm:ss} {$"[{msg.Severity}]",-9} {msg.Source,-8} | {msg.Exception?.ToString() ?? msg.Message}";
            return Console.Out.WriteLineAsync(txt);
        }

        private static void OnLogAsync(object? obj, LogMessageEventArgs args)
        {
            string txt = $"{DateTime.Now,-8:hh:mm:ss} {$"[{args.Level}]",-9} {args.Source,-8} | {args.Exception?.ToString() ?? args.Message}";
            Console.WriteLine(txt);
        }

    }
}
