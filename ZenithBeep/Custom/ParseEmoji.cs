
using Discord;

namespace ZenithBeep.Custom
{
    public class ParseEmoji
    {
        public Emoji GetEmoji(string emoji)
        {
            if (Emoji.TryParse(emoji, out var result))
            {
                return result;
            }
            else
            {
                return null;
            }


        }

        public Emote GetEmote(string emote)
        {
            if (Emote.TryParse(emote, out var result))
            {
                return result;
            }
            else
            {
                return null;
            }

        }
    }
}
