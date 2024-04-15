using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;
using ZenithBeep.Custom;

namespace ZenithBeep.Player
{
    public class MusicZenithHelper
    {
        private readonly IAudioService _audioService;
        public MusicZenithHelper(IAudioService audioService) {
            _audioService = audioService;
        }


        public async ValueTask<ZenithPlayer> GetPlayerAsync(SocketInteractionContext ctx, bool connectToVoiceChannel = true)
        {
            var channelBehavior = connectToVoiceChannel ? PlayerChannelBehavior.Move : PlayerChannelBehavior.None;
            var member = ctx.Client.GetMember(ctx.User.Id, ctx.Guild.Id);
            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior, VoiceStateBehavior: MemberVoiceStateBehavior.Ignore);
            var voice = member.VoiceState;

            var result = await _audioService.Players
                .RetrieveAsync<ZenithPlayer, ZenithPlayerOptions>(
                    guildId: ctx.Guild.Id,
                    memberVoiceChannel: member.VoiceState?.VoiceChannel?.Id,
                    playerFactory: ZenithPlayer.CreatePlayerAsync,
                    options: Options.Create(new ZenithPlayerOptions()
                    {
                        VoiceChannelId = member.VoiceState?.VoiceChannel?.Id,
                    }),
                    retrieveOptions: retrieveOptions
                );

            if (!result.IsSuccess)
            {

                var errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.Success => "Success",
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel",
                    PlayerRetrieveStatus.VoiceChannelMismatch => "You are not in the same channel as the Music Bot!",
                    PlayerRetrieveStatus.UserInSameVoiceChannel => "Same voice channel?",
                    PlayerRetrieveStatus.BotNotConnected => "The bot is currenly not connected.",
                    PlayerRetrieveStatus.PreconditionFailed => "A unknown error happened: Precondition Failed.",
                    _ => "A unknown error happened"
                };
                await ctx.Interaction.FollowupAsync(embed: CustomEmbeds.ErrorEmbed($"**{errorMessage}**"));
                return null;
            }


            return result.Player;
        }


        static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(properties);

            var player = new ZenithPlayer(properties);
            return ValueTask.FromResult(player);
        }

        
    }
}
