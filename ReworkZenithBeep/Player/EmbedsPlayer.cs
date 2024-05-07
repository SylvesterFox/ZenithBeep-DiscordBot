

using DSharpPlus;
using DSharpPlus.Entities;

namespace ReworkZenithBeep.Player
{
    public static class EmbedsPlayer
    {
        public static List<DiscordEmbedBuilder> QueueEmbed(ZenithPlayer player)
        {
            List<DiscordEmbedBuilder> embedBuilders = player.GetQueuePaged(10).Select(str => new DiscordEmbedBuilder().WithDescription(Formatter.BlockCode(str, "cs"))).ToList();

            return embedBuilders;
        }
    }
}
