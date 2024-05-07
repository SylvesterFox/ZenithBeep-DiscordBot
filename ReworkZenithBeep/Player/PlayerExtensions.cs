

using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players.Vote;

namespace ReworkZenithBeep.Player
{
    public static class PlayerExtensions
    {

        public static IEnumerable<string> GetQueuePaged(this VoteLavalinkPlayer player, int items)
        {
            List<string> pages = new List<string>();
            List<string> lines = new List<string>();
            int count = 0;

            foreach (TrackQueueItem tqi in player.Queue)
            {
                string line = $"{++count}. {tqi.Reference.Track.Title}";

                if (count % (items + 1) == 0)
                {
                    lines.Reverse();
                    pages.Add(string.Join("\n", lines));
                    lines.Clear();
                }

                lines.Add(line);
            }

            lines.Reverse();
            pages.Add(string.Join('\n', lines));
            return pages;
        }
    }
}
