

using System.Text.RegularExpressions;

namespace ReworkZenithBeep.Settings
{
    public interface IEmojiParser
    {
        string GetName();
        ulong GetId();
        bool IsAnimated();
    }

    public class EmojiParser : IEmojiParser
    {
        private static readonly string EmojiPattern = @"<(a?):(\w+):(\d+)>";
        private readonly bool isAnimated;
        private readonly string name;
        private readonly ulong id;

        public EmojiParser(string emojiStr)
        {
            var match = Regex.Match(emojiStr, EmojiPattern);
            if (match.Success)
            {
                isAnimated = match.Groups[1].Value == "a";
                name = match.Groups[2].Value;
                id = Convert.ToUInt64(match.Groups[3].Value);
            } else
            {
                throw new ArgumentException("Invalid emoji format");
            }
        }

        public string GetName() => name;

        public ulong GetId() => id;
        public bool IsAnimated() => isAnimated;

    }

}
