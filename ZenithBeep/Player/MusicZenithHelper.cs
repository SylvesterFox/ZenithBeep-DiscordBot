using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;

namespace ZenithBeep.Player
{
    public class MusicZenithHelper
    {
        private readonly IAudioService _audioService;
        public MusicZenithHelper(IAudioService audioService) {
            _audioService = audioService;
        }
        static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(properties);

            var player = new ZenithPlayer(properties);
            return ValueTask.FromResult(player);
        }

        

        public string GetPlayerErrorMessage(PlayerRetrieveStatus status)
        {
            var errorMessage = status switch
            {
                PlayerRetrieveStatus.Success => "Success",
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel",
                PlayerRetrieveStatus.VoiceChannelMismatch => "You are not in the same channel as the Music Bot!",
                PlayerRetrieveStatus.UserInSameVoiceChannel => "Same voice channel?",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currenly not connected.",
                PlayerRetrieveStatus.PreconditionFailed => "A unknown error happened: Precondition Failed.",
                _ => "A unknown error happened"
            };

            return errorMessage;
        }
    }
}
