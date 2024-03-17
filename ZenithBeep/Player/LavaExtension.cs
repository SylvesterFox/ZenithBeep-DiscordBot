using Discord;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;

namespace ZenithBeep.Player
{
    public static class LavaExtension
    {
        public const string DEFAULT_THUMBNAIL = "https://cdn.discordapp.com/attachments/617626417718624276/1218288659749404695/drgn_music.png?ex=66071ebb&is=65f4a9bb&hm=a7c79a8a8c6577a164b02b0675515b9257b7c0ec1d8d43b5dbfa5afc1e714423&";
        private static async ValueTask<string> GetThumbnail(this LavalinkTrack? track)
        {

            if (track is { SourceName: { } })
            {
                return track.ArtworkUri is null ? DEFAULT_THUMBNAIL : track.ArtworkUri.ToString();
            }

            return DEFAULT_THUMBNAIL;
        }

        public static async Task<Embed> GetEmbedAsync(this LavalinkTrack track, string prefix = null, Color? color = null, string defaultThumbnail = null)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                ThumbnailUrl = await track.GetThumbnail() ?? defaultThumbnail ?? DEFAULT_THUMBNAIL,
                Color = color ?? Color.Purple
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


            return builder.Build();

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
    }
}
