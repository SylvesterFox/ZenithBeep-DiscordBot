using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrechkaBOT
{
    public class GrechkaResult : RuntimeResult
    {

        public string Message { get; }

        private GrechkaResult(InteractionCommandError? error, string reason, string message) : base(error, reason)
        {
            Message = message;
        }

        public static GrechkaResult FromError(string reason, string message)
        {
            return new GrechkaResult(InteractionCommandError.Exception, reason, message);
        }

        public static GrechkaResult FromUserError(string reason, string message)
        {
            return new GrechkaResult(InteractionCommandError.ParseFailed, reason, message);
        }

        public static GrechkaResult FromSuccess(string reason = null, string message = null)
        {
            return new GrechkaResult(null, reason, message);
        }

    }
}
