using Discord;
using Lavalink4NET.Artwork;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrechkaBOT.Custom.DragonPlayer;

namespace GrechkaBOT.Custom
{
    public static class LavaLinkExtension
    {
        public const string DEFAULT_THUMBNAIL = "https://i.imgur.com/MzZzdO4.png";
        private static async ValueTask<string> GetThumbnail(this LavalinkTrack? track)
        {
            using ArtworkService artwork = new ArtworkService();

            if (track is { SourceName: { } })
            {
                Uri url = await artwork.ResolveAsync(track);
                return url is null ? DEFAULT_THUMBNAIL : url.ToString();
            }

            return DEFAULT_THUMBNAIL;
        }

        public static async Task<Embed> GetEmbedAsync(this LavalinkTrack track, string prefix = null, Color? color = null, string defaultThumbnail = null)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                ThumbnailUrl = await track.GetThumbnail() ?? (defaultThumbnail ?? DEFAULT_THUMBNAIL),
                Color = color ?? Color.Purple
            };

            EmbedAuthorBuilder embedAuthorBuilder = new EmbedAuthorBuilder()
            {
                Name = prefix is null ? track?.Title : $"{prefix}: {track?.Title}",
                IconUrl = "https://cdn.discordapp.com/attachments/918184699405426738/1147902100416766043/music.png",
                Url = $"{track.Uri}"
            };

            builder.WithAuthor(embedAuthorBuilder);
            builder.AddField("Author", track?.Author, true);
            builder.AddField("Duration", track.IsSeekable ? $"{(int)track.Duration.TotalMinutes}:{track.Duration.Seconds:00}" : "∞", true);
            

            if (track.Context is QueueInfo info)
            {
                builder.WithFooter($"Queued by: {info.User.Username}", info.User.GetAvatarUrl(size: 64));
            }

            return builder.Build();

        }
    }


}
