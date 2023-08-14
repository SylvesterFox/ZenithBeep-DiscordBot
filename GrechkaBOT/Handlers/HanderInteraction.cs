
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace GrechkaBOT.Handlers
{
    public class HanderInteraction
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _command;
        private readonly IServiceProvider _service;

        public HanderInteraction(DiscordSocketClient client, InteractionService command, IServiceProvider service)
        {
            _client = client;
            _command = command;
            _service = service;
        }

        public async Task InitializeAsync()
        {
            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

            _client.InteractionCreated += HandlerInteraction;

            _command.ContextCommandExecuted += ContextCommandExecuted;
            _command.SlashCommandExecuted += SlashCommandExecuted;
            _command.ComponentCommandExecuted += ComponentCommandExecuted;
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
                Console.WriteLine(ex);

                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
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

        private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

    }
}
