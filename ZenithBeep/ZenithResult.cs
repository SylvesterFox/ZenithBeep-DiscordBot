using Discord.Interactions;

namespace ZenithBeep
{
    public class ZenithResult : RuntimeResult
    {

        public string Message { get; }

        private ZenithResult(InteractionCommandError? error, string reason, string message) : base(error, reason)
        {
            Message = message;
        }

        public static ZenithResult FromError(string reason, string message)
        {
            return new ZenithResult(InteractionCommandError.Exception, reason, message);
        }

        public static ZenithResult FromUserError(string reason, string message)
        {
            return new ZenithResult(InteractionCommandError.ParseFailed, reason, message);
        }

        public static ZenithResult FromSuccess(string reason = null, string message = null)
        {
            return new ZenithResult(null, reason, message);
        }

    }
}
