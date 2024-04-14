using System.Web;
using Discord;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;
using ZenithBeep.ResourcesBot;

namespace ZenithBeep.Player
{
    public static class PlayerExtensions
    {
        public const string DEFAULT_THUMBNAIL = "https://cdn.discordapp.com/attachments/617626417718624276/1218288659749404695/drgn_music.png?ex=66071ebb&is=65f4a9bb&hm=a7c79a8a8c6577a164b02b0675515b9257b7c0ec1d8d43b5dbfa5afc1e714423&";
        private static async ValueTask<string> GetThumbnail(this LavalinkTrack? track)
        {
            string thubnail = DEFAULT_THUMBNAIL;

            bool isYoutubeUrl = (track.Uri?.Host == "youtube.com" || track.Uri?.Host == "www.youtube.com");

            if (isYoutubeUrl && track.Uri != null)
            {
                var uriQuery = HttpUtility.ParseQueryString(track.Uri.Query);
                var videoId = uriQuery["v"];

                thubnail = $"https://img.youtube.com/vi/{videoId}/0.jpg";
                return thubnail;
            }

            return DEFAULT_THUMBNAIL;
        }

        public static async Task<Embed> GetEmbedAsync(this LavalinkTrack track, string defaultThumbnail = null)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                ThumbnailUrl = await track.GetThumbnail() ?? defaultThumbnail ?? DEFAULT_THUMBNAIL,
                Color = Color.Magenta,
                Url = $"https://youtube.com/watch?v={track.Identifier}"
            };

            EmbedAuthorBuilder embedAuthorBuilder = new EmbedAuthorBuilder()
            {
                Name = $"Playing: {track?.Title}",
                IconUrl = DEFAULT_THUMBNAIL,
                Url = $"https://youtube.com/watch?v={track.Identifier}"
            };

            builder.WithAuthor(embedAuthorBuilder);
            builder.AddField("Author", track?.Author, true);
            builder.AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"), true);

            builder.WithFooter(x => {
                x.Text = $"ZenithBeep{Others.VERSION.Replace("Branch: ", "• ")} • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });

            return builder.Build();

        }

        public static Tuple<Embed, MessageComponent> SearchEmbed(LavalinkTrack[] tracks)
        {
            var embedBuilder = new EmbedBuilder()
            {
                Color = Color.DarkPurple,
                Title = "Result search:"
            };
            string desc = string.Empty;
            int item = 0;
            var buttons = new ComponentBuilder();
            foreach (LavalinkTrack track in tracks)
            {
                var count = ++item;
                desc += $"`{count}`. **{track.Title}**\n";
                buttons.WithButton($"{count}", $"btn_srch_{count}");
            }
            embedBuilder.WithDescription(desc);

           

            embedBuilder.WithFooter(x => {
                x.Text = $"ZenithBeep{Others.VERSION.Replace("Branch: ", "• ")} • SylvesterNotCute © Все права задраконины";
                x.WithIconUrl("https://avatars.githubusercontent.com/u/51517881?v=4");
            });



            return Tuple.Create(embedBuilder.Build(), buttons.Build());
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
