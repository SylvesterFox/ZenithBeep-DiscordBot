using Discord.WebSocket;
using Discord;


namespace GrechkaBOT.Handlers
{
    public class HandlerStatus
    {
        private Timer _timer;
        private readonly DiscordSocketClient _client;

        private readonly List<string> _statusList = new List<string> { "Драколис не милый", "трусиках пальчиками", "никчёмную жизнь" };
        private int _statusIndex = 0;
        public HandlerStatus(DiscordSocketClient client)
        {
            _client = client;

        }

        public async Task InitializeAsync()
        {
            _client.Ready += OnStatusBot;
        }

        private Task OnStatusBot()
        {
            _timer = new Timer(async _ =>
            {
                await _client.SetGameAsync(_statusList.ElementAtOrDefault(_statusIndex), type: ActivityType.Playing);
                _statusIndex = _statusIndex + 1 == _statusList.Count ? 0 : _statusIndex + 1;
            },
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }
    }
}
