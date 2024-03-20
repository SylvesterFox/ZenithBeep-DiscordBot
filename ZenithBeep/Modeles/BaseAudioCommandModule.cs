using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Players;
using ZenithBeep.Player;

namespace ZenithBeep.Modeles
{
    public class BaseAudioCommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        protected readonly IAudioService _audioService;
        protected readonly MusicZenithHelper _musicZenithHelper;

        public BaseAudioCommandModule(IAudioService audioService, MusicZenithHelper musicZenithHelper)
        {
            _audioService = audioService;
            _musicZenithHelper = musicZenithHelper;
        }

        protected async ValueTask<(PlayerResult<ZenithPlayer>, bool isPlayerConnected)> GetPlayerAsync(ulong guildId, ulong? voicechannel = null, bool connectToVoiceChannel = true)
        {
            return await _musicZenithHelper.GetPlayerAsync(guildId, voicechannel, connectToVoiceChannel);
        }

        protected string GetPlayerErrorMessage(PlayerRetrieveStatus status) => _musicZenithHelper.GetPlayerErrorMessage(status);
    }
}
