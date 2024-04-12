

namespace ZenithBeep.Settings
{
    public class BotConfig
    {
       
        public string TOKEN { get; set; } = string.Empty;
        public string LAVALINK_ADDRES { get; set; } = "http://localhost:2333";
        public string LAVALINK_WEBSOCKET { get; set; } = "ws://localhost:2333/v4/websocket";
        public string LAVALINK_PASSWORD { get; set; } = "youshallnotpass";

        public string LOGS { get; set; } = "info";

        public string POSTGRES_HOST { get; set; } = "localhost";
        public string POSTGRES_DB { get; set; } = "ZenitBeep";
        public string POSTGRES_PORT { get; set; } = "5432";
        public string POSTGRES_USER { get; set; } = "postgres";
        public string POSTGRES_PASSWORD { get; set; } = string.Empty;

        public bool AUDIOSERICES { get; set; } = false;
        public bool NODB_MODE { get; set; } = false;

    }
}
