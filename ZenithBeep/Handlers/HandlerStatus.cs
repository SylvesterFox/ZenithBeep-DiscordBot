using Discord.WebSocket;
using Discord;
using System.Text.Json;


namespace GrechkaBOT.Handlers
{

    public class HandlerStatus
    {
        private Timer _timer;
        private readonly DiscordSocketClient _client;
        public class Status
        {
            public string[]? status { get; set; }
        }


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
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "Resources/StatusContext.json";
            string jsonString = File.ReadAllText(fileName);
            

            Status? _statuslist = JsonSerializer.Deserialize<Status>(jsonString)!;
            int _count = _statuslist.status.Length;
            _timer = new Timer(async _ =>
            {
                await _client.SetGameAsync(_statuslist.status.ElementAtOrDefault(_statusIndex), type: ActivityType.Playing);
                _statusIndex = _statusIndex + 1 == _count ? 0 : _statusIndex + 1;
            },
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }
    }
}
