

using DSharpPlus;
using DSharpPlus.Entities;
using Lavalink4NET.Tracks;
using System.Web;


namespace ReworkZenithBeep.Player
{
    public static class EmbedsPlayer
    {
        public static string DEFAULT_THUMBNAIL = "https://cdn.discordapp.com/attachments/617626417718624276/1218288659749404695/drgn_music.png?ex=66071ebb&is=65f4a9bb&hm=a7c79a8a8c6577a164b02b0675515b9257b7c0ec1d8d43b5dbfa5afc1e714423&";

        private static string GetThumbnail(this LavalinkTrack track)
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

        public static List<DiscordEmbedBuilder> QueueEmbed(ZenithPlayer player)
        {
            List<DiscordEmbedBuilder> embedBuilders = player.GetQueuePaged(10).Select(str => new DiscordEmbedBuilder().WithDescription(Formatter.BlockCode(str, "cs"))).ToList();

            return embedBuilders;
        }


        public static async Task<DiscordEmbed> NowPlayingEmbed(this LavalinkTrack track, string? prefix = null, string? color = null)
        {
            

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor(color ?? "#800080"),
                Url = $"https://youtube.com/watch?v={track.Identifier}"
            };
            builder.WithThumbnail(GetThumbnail(track));
            builder.WithAuthor(
                name: prefix is null ? track?.Title : $"{prefix}: {track?.Title}",
                iconUrl: DEFAULT_THUMBNAIL,
                url: $"{track?.Uri}"
            );
            builder.AddField("Author", track?.Author, true);
            builder.AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"), true);

            builder.WithFooter("ZenithBeep v0.01[DSharpVer] • SylvesterNotCute © Все права задраконины", "https://avatars.githubusercontent.com/u/51517881?v=4");

            return builder;

        }


        public static DiscordEmbed EmptyQueueEmbed() => new DiscordEmbedBuilder().WithDescription("Nothing is playing!").Build();
    }
}
