
using Discord;

namespace ZenithBeep.Custom
{
    public class ParseEmoji
    {
        private static Emoji? _emoji = null;
        private static Emote? _emote = null;

        public Emoji? GetEmoji(string emoji)
        {
            if (Emoji.TryParse(emoji, out var result))
            {
                return result;
            }

            return null;


        }

        public Emote? GetEmote(string emote)
        {
            if (Emote.TryParse(emote, out var result))
            {
                return result;
            }

            return null;

        }

        public IEmote? GetParseEmoji(string emoji) 
        {
            _emote = GetEmote(emoji);
            if (_emote != null)
            {
                return _emote;
            }

            _emoji = GetEmoji(emoji);
            if ( _emoji != null)
            {
                return _emoji;
            }

            return null;
        }

        public string? GetNameEmoji(string emoji) 
        {
            var resultEmote = GetEmote(emoji);
            if (resultEmote !=  null)
            {
                return $"<:{resultEmote.Name}:{resultEmote.Id}>";
            }

            var result = GetEmoji(emoji);
            if ( result != null )
            {
                return result.Name;
            }

            return null;
        }
           
    }
}
