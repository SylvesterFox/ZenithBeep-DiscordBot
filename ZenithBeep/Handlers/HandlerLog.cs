using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace ZenithBeep.Handlers
{
    /// <summary>
    /// Logs Handler
    /// </summary>
    public class HandlerLog
    {
        public HandlerLog(DiscordSocketClient client, CommandService commandService)
        {
            client.Log += LogAsync;
            commandService.Log += LogAsync;
        }

        public Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $"failed to execute in {cmdException.Context.Channel}");

                Console.WriteLine(cmdException);
            } else
            {
                Console.WriteLine($"[General/{message.Severity}] {message}");
            }

            return Task.CompletedTask;
        }
    }
}
