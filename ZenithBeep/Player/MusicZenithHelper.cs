using Discord;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Players;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using ZenithBeepData;
using ZenithBeepData.Models;

namespace ZenithBeep.Player
{
    public class MusicZenithHelper
    {
        private readonly IAudioService _audioService;
        private readonly DataAccessLayer DataAccessLayer;
        private readonly DiscordSocketClient discordClient;
        internal Dictionary<ulong, GuildState> GuildStates { get; set; } = new Dictionary<ulong, GuildState>();
        public MusicZenithHelper(IAudioService audioService, DataAccessLayer dataAccessLayer, DiscordSocketClient client) {
            _audioService = audioService;
            DataAccessLayer = dataAccessLayer;
            discordClient = client;
        }

        static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(properties);

            var player = new ZenithPlayer(properties);
            return ValueTask.FromResult(player);
        }

        public async ValueTask<(PlayerResult<ZenithPlayer>, bool isPlayerConnected)> GetPlayerAsync(ulong guildId, ulong? voiceChannelId = null, bool connectToVoiceChannel = true)
        {
            var channelBehavior = connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None;

            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

            var result = await _audioService.Players
                .RetrieveAsync<ZenithPlayer, ZenithPlayerOptions>(
                    guildId: guildId,
                    memberVoiceChannel: voiceChannelId,
                    playerFactory: CreatePlayerAsync,
                    options: Options.Create(new ZenithPlayerOptions()
                    {
                        VoiceChannelId = voiceChannelId,
                    }),
                    retrieveOptions: retrieveOptions
                );

            if (!result.IsSuccess)
            {
                return (result, false);
            }

            bool isConnected = result.IsSuccess && result.Player != null && result.Player.State != PlayerState.Destroyed;

            return (result, isConnected);

        /*    if (result.Player != null && connectToVoiceChannel)
            {
                if (voiceChannelId != null)
                {

                }
            }*/
        }

        public async Task<IMessageChannel?> GetMusicTextChannelFor(SocketGuild guild)
        {
            var dbGuild = await DataAccessLayer.GetOrCreateGuild(guild);

            ulong? ChannelId = null;
            if (dbGuild.MusicChannelId == null)
            {
                if (GuildStates.ContainsKey(guild.Id) && GuildStates[guild.Id].TemporaryMusicChannelId != null)
                    ChannelId = GuildStates[guild.Id].TemporaryMusicChannelId;
            }
            else ChannelId = dbGuild.MusicChannelId;

            if (ChannelId == null) return null;
            return (IMessageChannel?)await discordClient.GetChannelAsync(ChannelId.Value);
        }

        public async Task<bool> DeletePastStatusMessage(ModelGuild guild, IMessageChannel outputChannel)
        {
            try
            {
                if (guild.LastMessageStatusId != null && outputChannel != null)
                {
                    ulong lastMessageStatusId = guild.LastMessageStatusId.Value;
                    var oldMessage = await outputChannel.GetMessageAsync(lastMessageStatusId);
                    if (oldMessage != null)
                    {
                        guild.LastMessageStatusId = null;

                        await oldMessage.DeleteAsync();
                        return true;
                    }
                }
            }
            catch { }

            return false;
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

        public void AnnounceJoin(ulong guildId, ulong channelId)
        {
            if (!GuildStates.ContainsKey(guildId))
                GuildStates[guildId] = new GuildState(channelId);

            if (GuildStates[guildId].TemporaryMusicChannelId == null)
                GuildStates[guildId].TemporaryMusicChannelId = channelId;
        }

        public void AnnoounceLeave(ulong guildId)
        {
            if (GuildStates.ContainsKey(guildId))
                GuildStates.Remove(guildId);
        }

        internal GuildState GetOrCreateGuildState(ulong guildId)
        {
            if (this.GuildStates.ContainsKey(guildId))
                return GuildStates[guildId];

            GuildStates.Add(guildId, new GuildState(guildId));
            return GuildStates[guildId];
        }


    }
}
