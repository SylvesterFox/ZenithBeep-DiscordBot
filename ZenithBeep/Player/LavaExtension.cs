using Discord;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;
using System.Web;
using TimeSpanParserUtil;
using ZenithBeepData.Models;

namespace ZenithBeep.Player
{
    public static class LavaExtension
    {
        public const string DEFAULT_THUMBNAIL = "https://cdn.discordapp.com/attachments/617626417718624276/1218288659749404695/drgn_music.png?ex=66071ebb&is=65f4a9bb&hm=a7c79a8a8c6577a164b02b0675515b9257b7c0ec1d8d43b5dbfa5afc1e714423&";
        private static async ValueTask<string> GetThumbnail(this LavalinkTrack? track)
        {
            string thumbnail = DEFAULT_THUMBNAIL;

            bool isYoutubeUrl = (track.Uri?.Host == "youtube.com" || track.Uri?.Host == "www.youtube.com");
            if (isYoutubeUrl && track.Uri != null)
            {
                var uriQuery = HttpUtility.ParseQueryString(track.Uri.Query);
                var videoId = uriQuery["v"];

                thumbnail = $"https://img.youtube.com/vi/{videoId}/0.jpg";
                return thumbnail;
            }

            return thumbnail;
        }

        public static async Task<EmbedBuilder> MusicEmbed(this LavalinkTrack track, string prefix = null, Color? color = null, ModelGuildQueueItem? dbTrack = null, string requestedBy = null)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                ThumbnailUrl = await track.GetThumbnail(),
                Color = color ?? Color.Purple,
                Url = $"https://youtube.com/watch?v={track.Identifier}"
            };

            EmbedAuthorBuilder embedAuthorBuilder = new EmbedAuthorBuilder()
            {
                Name = prefix is null ? track?.Title : $"{prefix}: {track?.Title}",
                IconUrl = DEFAULT_THUMBNAIL,
                Url = $"{track.Uri}"
            };

            builder.WithAuthor(embedAuthorBuilder);
            builder.AddField("Author", track?.Author, true);
            builder.AddField("Duration", track.IsSeekable ? $"{(int)track.Duration.TotalMinutes}:{track.Duration.Seconds:00}" : "∞", true);
            if (dbTrack == null)
            {
                builder.AddField("Position", "<TRX Nil>", true);
            }
            else builder.AddField("Positon", dbTrack.Position.ToString(), true);

            builder.AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"), true);
            builder.AddField("Requested by", requestedBy, true);

            builder.WithFooter(x => {
                x.Text = "ZenithBeep v1.08[DEV] • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });

            return builder;

        }

        public static IEnumerable<string> GetQueuePaged(this VoteLavalinkPlayer player, int items)
        {
            List<string> pages = new List<string>();
            List<string> lines = new List<string>();
            int count = 0;

            foreach (LavalinkTrack track in player.Queue)
            {
                string line = $"{++count}. {track.Title}";

                if (count % (items + 1) == 0)
                {
                    lines.Reverse();
                    pages.Add(string.Join("\n", lines));
                    lines.Clear();
                }

                lines.Add(line);
            }

            lines.Reverse();
            pages.Add(string.Join("\n", lines));
            return pages;
        }

        public static TimeSpan? TryParseTimeStamp(this string input)
        {
            TimeSpan ts;

            if (TimeSpan.TryParseExact(input, new string[] { "ss", "mm\\:ss", "mm\\-ss", "mm\\'ss", "mm\\;ss" }, null, out ts))
                return ts;

            if (TimeSpanParser.TryParse(input, timeSpan: out ts))
                return ts;

            return null;
        }
    }
}
