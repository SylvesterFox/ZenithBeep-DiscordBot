using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace ZenithBeep.Services
{
    public class LoggingService
    {
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;

        public LoggingService(IServiceProvider service) 
        {
            _client = service.GetRequiredService<DiscordSocketClient>();
            _logger = service.GetRequiredService<ILogger<LoggingService>>();

            _client.Log += OnLogAsync;
        }

        public Task OnLogAsync(LogMessage msg) 
        {
            string logText = $"{DateTime.Now,-8:hh:mm:ss} {$"[{msg.Severity}]",-9} {msg.Source,-8} | {msg.Exception?.ToString() ?? msg.Message}";

            switch (msg.Severity.ToString())
                {
                    case "Critical":
                    {
                        _logger.LogCritical(logText);
                        break;
                    }
                    case "Warning":
                    {
                        _logger.LogWarning(logText);
                        break;
                    }
                    case "Info":
                    {
                        _logger.LogInformation(logText);
                        break;
                    }
                    case "Verbose":
                    {
                        _logger.LogInformation(logText);
                        break;
                    } 
                    case "Debug":
                    {
                        _logger.LogDebug(logText);
                        break;
                    } 
                    case "Error":
                    {
                        _logger.LogError(logText);
                        break;
                    } 
                }

            return Task.CompletedTask;
        }

    }

    
}