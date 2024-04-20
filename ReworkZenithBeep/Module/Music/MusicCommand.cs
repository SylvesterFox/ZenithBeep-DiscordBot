

using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;
using ReworkZenithBeep.Player;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Music
{
    public partial class MusicCommand
    {
        private readonly IAudioService audioService;
        private static MusicCommand instance;

        private MusicCommand(IAudioService audioService)
        {
            ArgumentNullException.ThrowIfNull(audioService);
            this.audioService = audioService;
        }

        

        public static MusicCommand GetInstance(IAudioService audioService)
        {
            if (instance == null)
            {
                instance = new MusicCommand(audioService);
            }
            return instance;
        }

        private async ValueTask<ZenithPlayer?> GetPlayerAsync(CommonContext ctx, bool connectToVoiceChannel = true)
        {
            var options = new ZenithPlayerOptions() { Context = ctx, SelfDeaf = true, HistoryCapacity = 10 };
            var retrieveOptions = new PlayerRetrieveOptions(
                    ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Move : PlayerChannelBehavior.None,
                    VoiceStateBehavior: MemberVoiceStateBehavior.Ignore);

            PlayerResult<ZenithPlayer> result;
            try
            {
                result = await audioService.Players
                    .RetrieveAsync<ZenithPlayer, ZenithPlayerOptions>(ctx.Guild.Id, ctx.Member.VoiceState?.Channel?.Id,
                    ZenithPlayer.CreatePlayerAsync,
                    Options.Create(options),
                    retrieveOptions
                    ).ConfigureAwait(false);
            } catch (TimeoutException)
            {
                await ctx.RespondTextAsync("Timeout player error");
                return null;
            }

            if (!result.IsSuccess)
            {
                var message = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel",
                    _ => "A unknown error happened"
                };
            }

            return result.Player;
        }

        public async Task JoinAsync(CommonContext context)
        {  
            var player = await GetPlayerAsync(context); if (player == null) return;
            var voiceChannel = context.Guild.GetChannel(player.VoiceChannelId);
            await context.RespondTextAsync($"Connect to `{voiceChannel.Name}`").ConfigureAwait(false);
        }

        public async Task LeaveAsync(CommonContext context)
        {
            var player = await GetPlayerAsync(context, false);
            if (player == null) return;
            var voiceChannel = context.Guild.GetChannel(player.VoiceChannelId);

            await player.DisconnectAsync();
            await player.DisposeAsync();

            await context.RespondTextAsync($"Leave from `{voiceChannel.Name}`. Bye!");
        }
    }
}
