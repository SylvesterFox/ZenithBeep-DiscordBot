using Discord;
using Discord.WebSocket;
using static ZenithBeep.Player.YoutubeParser;


namespace ZenithBeep.Player
{
    internal class GuildState
    {
        public ulong GuildId { get; set; }

        public ulong? TemporaryMusicChannelId { get; set; } = null;

        public bool SetNextFlag { get; set; } = false;
        public bool ShuffleEnbled { get; set; } = false;
        public bool RepeatEnabled { get; set; } = false;
        public int TimesRepeated { get; set; } = 0;

        public MusicEmbedState? MusicEnbed { get; set; } = null;

        public SortedList<TimeSpan, IVideoChapter>? TrackChapters { get; set; }

        public GuildState(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    internal class MusicEmbedState
    {
        public EmbedBuilder Embed { get; set; }
        public int ProgressFieldIdx { get; set; }

        public SocketMessage Message { get; set; }
    }
}
