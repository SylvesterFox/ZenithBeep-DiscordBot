using Discord;
using Discord.WebSocket;
using Lavalink4NET.Tracks;


namespace ZenithBeep.Player
{
    public static class PlayerEmbed
    {
        public static Tuple<Embed, MessageComponent> SearchEmbed(LavalinkTrack[] tracks)
        {
            var embedBuilder = new EmbedBuilder()
            {
                Color = Color.DarkPurple,
                Title = "Result search:"
            };
            string desc = string.Empty;
            for (int i = 0; i <= 4; i++)
            {
                desc += $"{i + 1}. {tracks[i].Title}\n";
            }
            embedBuilder.WithDescription(desc);

            var buttons = new ComponentBuilder()
                .WithButton("1", "btn_srch_1", ButtonStyle.Primary)
                .WithButton("2", "btn_srch_2", ButtonStyle.Primary)
                .WithButton("3", "btn_srch_3", ButtonStyle.Primary)
                .WithButton("4", "btn_srch_4", ButtonStyle.Primary)
                .WithButton("5", "btn_srch_5", ButtonStyle.Primary);
            

            return Tuple.Create(embedBuilder.Build(), buttons.Build());
        }
    }
}
